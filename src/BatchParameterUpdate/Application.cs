using System;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using BatchParameterUpdate.Commands;

namespace BatchParameterUpdate;

/// <summary>
/// Entry point of the add-in. Revit instantiates this class at startup because the
/// manifest registers it with Type="Application". Its only job is to publish the
/// ribbon button that launches <see cref="BatchParameterUpdateCommand"/>.
/// </summary>
public class Application : IExternalApplication
{
    private const string TabName = "BIM Tools";
    private const string PanelName = "Parameters";

    public Result OnStartup(UIControlledApplication application)
    {
        CreateRibbon(application);
        return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication application)
    {
        return Result.Succeeded;
    }

    private static void CreateRibbon(UIControlledApplication application)
    {
        try
        {
            application.CreateRibbonTab(TabName);
        }
        catch (Autodesk.Revit.Exceptions.ArgumentException)
        {
            // Another add-in already created a tab with this name. Revit rejects the
            // duplicate, which is fine: the panel below is added to the existing tab.
        }

        RibbonPanel panel = application.CreateRibbonPanel(TabName, PanelName);

        var buttonData = new PushButtonData(
            name: "BatchParameterUpdateButton",
            text: "Batch\nUpdate",
            assemblyName: Assembly.GetExecutingAssembly().Location,
            className: typeof(BatchParameterUpdateCommand).FullName)
        {
            ToolTip = "Writes a text value into one instance parameter on every selected element.",
            LongDescription =
                "Select the elements first, then run the command and type the parameter name and the " +
                "new value. Elements that do not expose a writable text instance parameter under that " +
                "name are skipped and listed in the summary.",
            AvailabilityClassName = typeof(SelectionAvailability).FullName
        };

        var button = (PushButton) panel.AddItem(buttonData);
        button.Image = LoadIcon("RibbonIcon16.png");
        button.LargeImage = LoadIcon("RibbonIcon32.png");
    }

    /// <summary>
    /// Reads an icon from the WPF resources compiled into this assembly. The assembly name is
    /// spelled out in the pack URI so the image resolves without an active WPF application,
    /// which Revit never creates for an add-in.
    /// </summary>
    private static BitmapImage LoadIcon(string fileName)
    {
        var uri = new Uri(
            $"pack://application:,,,/BatchParameterUpdate;component/Resources/Icons/{fileName}",
            UriKind.Absolute);

        return new BitmapImage(uri);
    }
}
