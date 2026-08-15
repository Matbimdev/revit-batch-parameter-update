using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BatchParameterUpdate.Commands;

/// <summary>
/// Greys out the ribbon button while the command has nothing to work on. Revit calls this
/// on every UI refresh, so the check stays free of document queries.
/// </summary>
/// <remarks>
/// Revit fills <c>selectedCategories</c> with the categories of the current selection, so an
/// empty set means an empty selection. The rare element without a category would also produce
/// an empty set, which is why the command repeats the real check on the selected ids.
/// </remarks>
public class SelectionAvailability : IExternalCommandAvailability
{
    public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
    {
        if (applicationData.ActiveUIDocument?.Document is null) return false;

        return !selectedCategories.IsEmpty;
    }
}
