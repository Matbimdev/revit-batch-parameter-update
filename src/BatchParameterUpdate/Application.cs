using Nice3point.Revit.Toolkit.External;
using BatchParameterUpdate.Commands;

namespace BatchParameterUpdate;

/// <summary>
///     Application entry point
/// </summary>
[UsedImplicitly]
public class Application : ExternalApplication
{
    public override void OnStartup()
    {
        CreateRibbon();
    }

    private void CreateRibbon()
    {
        var panel = Application.CreatePanel("Commands", "BatchParameterUpdate");

        panel.AddPushButton<StartupCommand>("Execute")
            .SetImage("/BatchParameterUpdate;component/Resources/Icons/RibbonIcon16.png")
            .SetLargeImage("/BatchParameterUpdate;component/Resources/Icons/RibbonIcon32.png");
    }
}