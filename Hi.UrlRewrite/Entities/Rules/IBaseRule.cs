using Hi.UrlRewrite.Entities.Conditions;
using Hi.UrlRewrite.Entities.Match;

namespace Hi.UrlRewrite.Entities.Rules
{
    public interface IBaseRule : IConditionsProperties, IBaseMatch
    {
        bool Enabled { get; set; }
    }
}