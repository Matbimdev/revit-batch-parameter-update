using Autodesk.Revit.UI;

namespace BatchParameterUpdate;

/// <summary>
/// Entry point of the add-in. Revit instantiates this class at startup because the
/// manifest registers it with Type="Application".
/// </summary>
public class Application : IExternalApplication
{
    public Result OnStartup(UIControlledApplication application)
    {
        return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication application)
    {
        return Result.Succeeded;
    }
}
