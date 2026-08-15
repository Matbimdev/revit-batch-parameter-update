using BatchParameterUpdate.ViewModels;

namespace BatchParameterUpdate.Views;

public sealed partial class BatchParameterUpdateView
{
    public BatchParameterUpdateView(BatchParameterUpdateViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}