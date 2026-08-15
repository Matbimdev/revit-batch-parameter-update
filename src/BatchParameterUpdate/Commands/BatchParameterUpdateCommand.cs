using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BatchParameterUpdate.Commands;

/// <summary>
/// Writes a text value into one instance parameter across the elements the user selected
/// before running the command.
/// </summary>
/// <remarks>
/// The transaction attribute has to sit on this concrete class. Revit reads it by reflection
/// on the type named in the manifest and does not look at base types.
/// </remarks>
[Transaction(TransactionMode.Manual)]
public class BatchParameterUpdateCommand : IExternalCommand
{
    internal const string CommandTitle = "Batch Parameter Update";

    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        UIDocument uiDocument = commandData.Application.ActiveUIDocument;
        if (uiDocument?.Document is null)
        {
            ShowMessage("Open a project before running this command.");
            return Result.Cancelled;
        }

        // The selection is read once, at start up. Everything that follows works on this
        // snapshot, so opening the input dialog cannot change the set of elements to update.
        ICollection<ElementId> selectedIds = uiDocument.Selection.GetElementIds();
        if (selectedIds.Count == 0)
        {
            ShowMessage("Select one or more elements before running the command.");
            return Result.Cancelled;
        }

        ShowMessage($"{selectedIds.Count} element(s) selected.");
        return Result.Succeeded;
    }

    /// <summary>
    /// Shows a short notice using the Revit task dialog. Reserved for messages that carry no
    /// data, the parameter input and the run summary get their own windows.
    /// </summary>
    internal static void ShowMessage(string text)
    {
        var dialog = new TaskDialog(CommandTitle)
        {
            MainInstruction = text,
            TitleAutoPrefix = false
        };

        dialog.Show();
    }
}
