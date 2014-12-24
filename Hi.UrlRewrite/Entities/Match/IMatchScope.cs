namespace Hi.UrlRewrite.Entities.Match
{
    public interface IMatchScope
    {
        ScopeType? MatchingScope { get; set; }
        string ScopeValue { get; set; }
    }
}
