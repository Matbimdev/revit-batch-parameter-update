using System.Collections.Generic;
using System.Linq;

namespace BatchParameterUpdate.Models;

/// <summary>
/// What one run of the batch did. Reported to the user as it is, with no further processing.
/// </summary>
public sealed class BatchUpdateResult
{
    public BatchUpdateResult(int updatedCount, IReadOnlyList<SkippedElement> skipped, bool modelChanged)
    {
        UpdatedCount = updatedCount;
        Skipped = skipped;
        ModelChanged = modelChanged;
    }

    public int UpdatedCount { get; }

    public IReadOnlyList<SkippedElement> Skipped { get; }

    public int SkippedCount => Skipped.Count;

    /// <summary>
    /// False when the transaction was rolled back, which happens when nothing was written.
    /// The document is then byte for byte what it was before the command ran.
    /// </summary>
    public bool ModelChanged { get; }

    /// <summary>
    /// The skipped elements grouped by cause, ordered by how many elements each cause affected,
    /// so the summary opens with the problem worth fixing first.
    /// </summary>
    public IEnumerable<IGrouping<SkipReason, SkippedElement>> GroupSkipsByReason()
    {
        return Skipped
            .GroupBy(element => element.Reason)
            .OrderByDescending(group => group.Count());
    }
}
