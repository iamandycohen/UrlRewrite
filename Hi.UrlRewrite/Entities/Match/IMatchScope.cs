using System.Collections.Generic;
namespace Hi.UrlRewrite.Entities.Match
{
    public interface IMatchScope
    {
        ScopeType? MatchingScope { get; set; }
        List<MatchTag> MatchTheContentWithin { get; set; }
    }
}
