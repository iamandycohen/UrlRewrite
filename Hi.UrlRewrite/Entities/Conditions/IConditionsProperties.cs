using System.Collections.Generic;
using Hi.UrlRewrite.Entities.Rules;

namespace Hi.UrlRewrite.Entities.Conditions
{
    public interface IConditionsProperties : IConditionLogicalGrouping
    {
        List<Condition> Conditions { get; set; }
    }
}