using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hi.UrlRewrite.Processing.Results
{
    public class OutboundRuleResult : IRuleResult

    {
        public ConditionMatchResult ConditionMatchResult { get; set; }
        public bool RuleMatched { get; set; }
        public bool StopProcessing { get; set; }
    }
}