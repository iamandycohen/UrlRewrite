using Hi.UrlRewrite.Entities.Rules;

namespace Hi.UrlRewrite.Entities.Match
{
    public interface IBaseMatch
    {
        MatchType? MatchType { get; set; }
        Using? Using { get; set; }
        string Pattern { get; set; }
        bool IgnoreCase { get; set; }
    }
}