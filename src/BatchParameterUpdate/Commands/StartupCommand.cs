using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using BatchParameterUpdate.ViewModels;
using BatchParameterUpdate.Views;

namespace BatchParameterUpdate.Commands;

/// <summary>
///     External command entry point.
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
        var viewModel = new BatchParameterUpdateViewModel();
        var view = new BatchParameterUpdateView(viewModel);
        view.ShowDialog();
    }
}