namespace BatchParameterUpdate.Models;

/// <summary>
/// Why an element was left untouched by the batch. Every skip carries one of these, so the
/// summary can tell the user what to fix instead of only how many elements failed.
/// </summary>
public enum SkipReason
{
    /// <summary>The id was in the selection but the document no longer resolves it.</summary>
    ElementNotFound,

    /// <summary>Another user holds the element in a workshared model.</summary>
    NotEditable,

    /// <summary>The element has no instance parameter under the name that was typed.</summary>
    ParameterNotFound,

    /// <summary>The parameter exists but stores something other than text.</summary>
    NotTextParameter,

    /// <summary>The parameter stores text but Revit does not allow writing to it.</summary>
    ReadOnlyParameter,

    /// <summary>Revit accepted the call but reported that the value was not applied.</summary>
    UpdateRejected,

    /// <summary>Revit refused the write and explained why.</summary>
    UpdateFailed
}

public static class SkipReasonDescription
{
    /// <summary>
    /// The heading used for this reason in the summary. Written for a Revit user, not for a
    /// developer, so it says what happened rather than naming the API that reported it.
    /// </summary>
    public static string ToDisplayText(this SkipReason reason)
    {
        switch (reason)
        {
            case SkipReason.ElementNotFound:
                return "Element no longer exists in the document";
            case SkipReason.NotEditable:
                return "Element is owned by another user";
            case SkipReason.ParameterNotFound:
                return "Parameter not found on the element";
            case SkipReason.NotTextParameter:
                return "Parameter does not store text";
            case SkipReason.ReadOnlyParameter:
                return "Parameter is read only";
            case SkipReason.UpdateRejected:
                return "Revit did not apply the value";
            case SkipReason.UpdateFailed:
                return "Revit refused the change";
            default:
                return "Unknown reason";
        }
    }
}
