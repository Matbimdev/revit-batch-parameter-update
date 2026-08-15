using System.Windows;

namespace BatchParameterUpdate.Views;

/// <summary>
/// Asks for the parameter to write and the value to write into it.
/// </summary>
/// <remarks>
/// Plain code behind rather than a view model. The window holds two strings, lives for the
/// length of one modal call and shares state with nothing, so the indirection of a binding
/// layer would buy nothing here.
/// </remarks>
public partial class ParameterInputWindow : Window
{
    public ParameterInputWindow(int selectedElementCount)
    {
        InitializeComponent();

        ScopeText.Text = selectedElementCount == 1
            ? "1 element is selected."
            : $"{selectedElementCount} elements are selected.";

        ParameterNameBox.Focus();
    }

    /// <summary>The parameter name typed by the user, trimmed of stray spaces.</summary>
    public string ParameterName => ParameterNameBox.Text.Trim();

    /// <summary>
    /// The value to write, taken as typed. It is not trimmed, because a user who types a
    /// trailing space may well mean it, and an empty string is a valid way to clear a
    /// parameter.
    /// </summary>
    public string NewValue => NewValueBox.Text;

    private void OnInputChanged(object sender, RoutedEventArgs e)
    {
        // A parameter has to be named for the command to have a target. The value does not,
        // so only this field gates the button.
        UpdateButton.IsEnabled = ParameterNameBox.Text.Trim().Length > 0;
    }

    private void OnUpdateClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }
}
