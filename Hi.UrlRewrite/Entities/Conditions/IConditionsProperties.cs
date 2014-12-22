using System.Collections.Generic;
using Hi.UrlRewrite.Entities.Rules;

namespace Hi.UrlRewrite.Entities.Conditions
{
    public interface IConditionsProperties
    {
        List<Condition> Conditions { get; set; }
        LogicalGrouping? ConditionLogicalGrouping { get; set; }
    }
}