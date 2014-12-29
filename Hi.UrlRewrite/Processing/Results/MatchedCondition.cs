using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hi.UrlRewrite.Entities.Conditions;

namespace Hi.UrlRewrite.Processing.Results
{
    public class MatchedCondition
    {
        public Condition Condition { get; set; }
        public string ConditionInput { get; set; }

        public MatchedCondition()
        {
                
        }
        public MatchedCondition(Condition condition, string conditionInput)
        {
            Condition = condition;
            ConditionInput = conditionInput;
        }
    }
}