using Hi.UrlRewrite.Entities.Rules;

namespace Hi.UrlRewrite.Entities.Conditions
{
    public interface IConditionLogicalGrouping
    {
        LogicalGrouping? ConditionLogicalGrouping { get; set; }
    }
}