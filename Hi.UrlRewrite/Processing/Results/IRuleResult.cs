using System;
using Hi.UrlRewrite.Entities.ServerVariables;

namespace Hi.UrlRewrite.Processing.Results
{
    interface IRuleResult : IServerVariableList
    {
        ConditionMatchResult ConditionMatchResult { get; set; }
        bool RuleMatched { get; set; }
        bool StopProcessing { get; set; }
    }
}
