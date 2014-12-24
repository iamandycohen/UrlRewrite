
namespace Hi.UrlRewrite.Entities.Match
{
    public interface IBaseMatch : IUsing
    {
        MatchType? MatchType { get; set; }
        string Pattern { get; set; }
        bool IgnoreCase { get; set; }
    }
}