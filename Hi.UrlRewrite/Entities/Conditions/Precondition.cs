using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hi.UrlRewrite.Entities.Match;
using Hi.UrlRewrite.Entities.Rules;

namespace Hi.UrlRewrite.Entities.Conditions
{
    [Serializable]
    public class Precondition : IUsing, IConditionsProperties, IConditionList
    {
        public string Name { get; set; }
        public Using? Using { get; set; }
        public LogicalGrouping? ConditionLogicalGrouping { get; set; }
        public IEnumerable<Condition> Conditions { get; set; }

        public Precondition()
        {
            Conditions = new List<Condition>();
        }
    }
}