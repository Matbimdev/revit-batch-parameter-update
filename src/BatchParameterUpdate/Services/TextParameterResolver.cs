using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using BatchParameterUpdate.Models;

namespace BatchParameterUpdate.Services;

/// <summary>
/// Decides which parameter of an element a batch run may write to, and explains itself when
/// there is none. Keeping this apart from the update loop keeps the loop readable and puts
/// every rule about what counts as a valid target in one place.
/// </summary>
public static class TextParameterResolver
{
    /// <summary>
    /// Returns the writable text parameter named <paramref name="parameterName"/> on this
    /// element, or null. On null, <paramref name="failureReason"/> says why.
    /// </summary>
    /// <remarks>
    /// GetParameters reads the parameters held by the element itself, which are its instance
    /// parameters, so type parameters never reach this method. The match is exact and case
    /// sensitive: treating "comments" as "Comments" would hide a typo by writing to a
    /// parameter the user did not name.
    ///
    /// One name can resolve to several parameters, for example a built in one and a shared one
    /// added by a template. The writable text candidate wins, and when none qualifies the
    /// reported reason describes the closest match rather than whichever entry came first.
    /// </remarks>
    public static Parameter? Resolve(Element element, string parameterName, out SkipReason failureReason)
    {
        IList<Parameter> candidates = element.GetParameters(parameterName);
        if (candidates.Count == 0)
        {
            failureReason = SkipReason.ParameterNotFound;
            return null;
        }

        List<Parameter> textCandidates = candidates
            .Where(candidate => candidate.StorageType == StorageType.String)
            .ToList();

        if (textCandidates.Count == 0)
        {
            failureReason = SkipReason.NotTextParameter;
            return null;
        }

        Parameter? writable = textCandidates.FirstOrDefault(candidate => !candidate.IsReadOnly);
        if (writable is null)
        {
            failureReason = SkipReason.ReadOnlyParameter;
            return null;
        }

        failureReason = default;
        return writable;
    }
}
