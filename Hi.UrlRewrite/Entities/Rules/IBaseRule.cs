using System;
using Hi.UrlRewrite.Entities.Conditions;
using Hi.UrlRewrite.Entities.Match;

namespace Hi.UrlRewrite.Entities.Rules
{
    public interface IBaseRule : IConditionsProperties, IBaseMatch, IBaseEnabled
    {
        Guid ItemId { get; set; }
        string Name { get; set; }
    }
}