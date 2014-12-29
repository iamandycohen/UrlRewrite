using System;
using System.Collections.Generic;
using Hi.UrlRewrite.Entities.Conditions;
using Hi.UrlRewrite.Entities.Rules;

namespace Hi.UrlRewrite.Processing.Results
{
    public class ConditionMatchResult
    {
        public bool Matched { get; set; }
        public List<MatchedCondition> MatchedConditions { get; set; }
        public LogicalGrouping LogincalGrouping { get; set; }

        public ConditionMatchResult()
        {
            MatchedConditions = new List<MatchedCondition>();
        }
    }
}