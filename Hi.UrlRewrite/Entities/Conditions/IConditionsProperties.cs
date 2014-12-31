using System.Collections.Generic;
using Hi.UrlRewrite.Entities.Rules;

namespace Hi.UrlRewrite.Entities.Conditions
{
    public interface IConditionsProperties : IConditionLogicalGrouping
    {
        IEnumerable<Condition> Conditions { get; set; }
    }
}