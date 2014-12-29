using System;
namespace Hi.UrlRewrite.Processing.Results
{
    interface IRuleResult
    {
        ConditionMatchResult ConditionMatchResult { get; set; }
        bool RuleMatched { get; set; }
        bool StopProcessing { get; set; }
    }
}
