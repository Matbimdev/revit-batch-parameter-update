using System.Windows;
using System.Windows.Interop;
using Autodesk.Revit.UI;

namespace BatchParameterUpdate.Views;

/// <summary>
/// Hands a dialog to Revit as its owner.
/// </summary>
public static class RevitWindowOwner
{
    /// <summary>
    /// Makes the Revit main window the owner of this dialog.
    /// </summary>
    /// <remarks>
    /// A WPF window opened from an add-in has no owner of its own, so clicking on Revit sends
    /// the dialog behind it. The dialog is modal, so Revit then ignores every click and looks
    /// frozen with no visible explanation. Giving it an owner keeps it in front of the window
    /// it is blocking.
    /// </remarks>
    public static void SetRevitOwner(this Window window, UIApplication application)
    {
        new WindowInteropHelper(window).Owner = application.MainWindowHandle;
    }
}
