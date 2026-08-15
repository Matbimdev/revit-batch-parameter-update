using System.Collections.Generic;
using Autodesk.Revit.DB;
using BatchParameterUpdate.Models;

namespace BatchParameterUpdate.Services;

/// <summary>
/// Writes one text value into one instance parameter across a set of elements, inside a single
/// transaction. Holds every change to the model made by this add-in.
/// </summary>
public sealed class ParameterUpdateService
{
    private const string TransactionName = "Batch Parameter Update";

    private readonly Document _document;

    public ParameterUpdateService(Document document)
    {
        _document = document;
    }

    /// <summary>
    /// Applies <paramref name="newValue"/> to the parameter named <paramref name="parameterName"/>
    /// on every element it can, and reports what happened to the rest.
    /// </summary>
    /// <remarks>
    /// One transaction covers the whole batch, so a single undo takes the model back to where it
    /// started. An element Revit refuses is recorded and the run continues, because the point of
    /// the command is to get through a selection rather than stop at the first awkward element.
    ///
    /// Anything thrown outside that per element handling means the run cannot be trusted, so the
    /// transaction is rolled back and the exception travels up to the caller. The transaction is
    /// also disposed by the using statement, which rolls back anything still open if a path ever
    /// escapes without committing.
    /// </remarks>
    public BatchUpdateResult Run(ICollection<ElementId> elementIds, string parameterName, string newValue)
    {
        var updatedCount = 0;
        var skipped = new List<SkippedElement>();

        using (var transaction = new Transaction(_document, TransactionName))
        {
            transaction.Start();

            try
            {
                foreach (ElementId elementId in elementIds)
                {
                    Element element = _document.GetElement(elementId);
                    if (element is null)
                    {
                        skipped.Add(SkippedElement.Missing(elementId));
                        continue;
                    }

                    Parameter? parameter = TextParameterResolver.Resolve(
                        element, parameterName, out SkipReason failureReason);

                    if (parameter is null)
                    {
                        skipped.Add(SkippedElement.For(element, failureReason));
                        continue;
                    }

                    Write(element, parameter, newValue, skipped, ref updatedCount);
                }
            }
            catch
            {
                transaction.RollBack();
                throw;
            }

            // Nothing was written, so there is no reason to leave an empty step in the undo
            // stack. Committing an empty transaction would also tell the user the model
            // changed when it did not.
            if (updatedCount == 0)
            {
                transaction.RollBack();
                return new BatchUpdateResult(updatedCount, skipped, modelChanged: false);
            }

            bool modelChanged = transaction.Commit() == TransactionStatus.Committed;
            return new BatchUpdateResult(updatedCount, skipped, modelChanged);
        }
    }

    private static void Write(
        Element element,
        Parameter parameter,
        string newValue,
        ICollection<SkippedElement> skipped,
        ref int updatedCount)
    {
        try
        {
            if (parameter.Set(newValue))
            {
                updatedCount++;
                return;
            }

            skipped.Add(SkippedElement.For(element, SkipReason.UpdateRejected));
        }
        catch (Autodesk.Revit.Exceptions.ApplicationException exception)
        {
            // Base type of every exception the Revit API raises. Catching it keeps the batch
            // going when Revit rejects one element, while a genuine defect in this add-in
            // still surfaces as an unhandled exception instead of being buried in the summary.
            skipped.Add(SkippedElement.For(element, SkipReason.UpdateFailed, exception.Message));
        }
    }
}
