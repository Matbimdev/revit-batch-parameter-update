using System.Collections.Generic;
using System.Linq;
using System.Windows;
using BatchParameterUpdate.Models;

namespace BatchParameterUpdate.Views;

/// <summary>
/// Reports what one run did: how many elements were updated, how many were skipped, and the
/// cause of every skip.
/// </summary>
public partial class ResultsWindow : Window
{
    /// <summary>
    /// How many elements are listed under one cause before the rest are summarised. A batch
    /// over a whole floor can skip thousands of elements, and a list that long tells the user
    /// nothing the count does not already say.
    /// </summary>
    private const int MaxListedPerReason = 25;

    public ResultsWindow(BatchUpdateResult result, string parameterName, string newValue)
    {
        InitializeComponent();

        HeadlineText.Text = $"{result.UpdatedCount} updated, {result.SkippedCount} skipped";
        DetailText.Text = DescribeRun(result, parameterName, newValue);
        SkipGroups.ItemsSource = BuildGroups(result);
        SkipScroller.Visibility = result.SkippedCount == 0 ? Visibility.Collapsed : Visibility.Visible;
    }

    private static string DescribeRun(BatchUpdateResult result, string parameterName, string newValue)
    {
        if (!result.ModelChanged)
        {
            return $"Nothing was written to \"{parameterName}\", so the model is unchanged.";
        }

        return newValue.Length == 0
            ? $"\"{parameterName}\" was cleared on the updated elements."
            : $"\"{parameterName}\" was set to \"{newValue}\" on the updated elements.";
    }

    private static IReadOnlyList<SkipGroup> BuildGroups(BatchUpdateResult result)
    {
        return result.GroupSkipsByReason()
            .Select(group => new SkipGroup(
                header: $"{group.Key.ToDisplayText()} ({group.Count()})",
                lines: BuildLines(group.ToList())))
            .ToList();
    }

    private static IReadOnlyList<string> BuildLines(IReadOnlyList<SkippedElement> elements)
    {
        List<string> lines = elements
            .Take(MaxListedPerReason)
            .Select(element => element.ToDisplayText())
            .ToList();

        int remaining = elements.Count - lines.Count;
        if (remaining > 0)
        {
            lines.Add($"and {remaining} more");
        }

        return lines;
    }

    /// <summary>One cause and the elements it affected, shaped for the template above.</summary>
    public sealed class SkipGroup
    {
        public SkipGroup(string header, IReadOnlyList<string> lines)
        {
            Header = header;
            Lines = lines;
        }

        public string Header { get; }

        public IReadOnlyList<string> Lines { get; }
    }
}
