using Autodesk.Revit.DB;

namespace BatchParameterUpdate.Models;

/// <summary>
/// One element the batch could not write to, with enough context for the user to find it in
/// the model. Updated elements need no record of their own, they are only counted.
/// </summary>
public sealed class SkippedElement
{
    private SkippedElement(ElementId id, string name, string category, SkipReason reason, string? detail)
    {
        Id = id;
        Name = name;
        Category = category;
        Reason = reason;
        Detail = detail;
    }

    public ElementId Id { get; }

    public string Name { get; }

    public string Category { get; }

    public SkipReason Reason { get; }

    /// <summary>Revit's own message, present only when Revit refused the change.</summary>
    public string? Detail { get; }

    public static SkippedElement For(Element element, SkipReason reason, string? detail = null)
    {
        return new SkippedElement(
            element.Id,
            ReadName(element),
            element.Category?.Name ?? "No category",
            reason,
            detail);
    }

    /// <summary>An id from the selection that the document no longer resolves.</summary>
    public static SkippedElement Missing(ElementId id)
    {
        return new SkippedElement(id, "Unknown", "No category", SkipReason.ElementNotFound, null);
    }

    /// <summary>
    /// One line identifying the element in the summary. ToString on the id prints its numeric
    /// value on every supported version, which avoids the int to long change made in 2025.
    /// </summary>
    public string ToDisplayText()
    {
        string line = $"{Category}: {Name} (id {Id})";
        return Detail is null ? line : $"{line}. {Detail}";
    }

    /// <summary>
    /// Not every element type supports Name, and the ones that do not throw when it is read.
    /// A missing name must not abort a batch that is otherwise fine.
    /// </summary>
    private static string ReadName(Element element)
    {
        try
        {
            return string.IsNullOrWhiteSpace(element.Name) ? "Unnamed" : element.Name;
        }
        catch (Autodesk.Revit.Exceptions.ApplicationException)
        {
            return "Unnamed";
        }
    }
}
