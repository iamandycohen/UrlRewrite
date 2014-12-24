using Hi.UrlRewrite.Entities.Actions;
using Hi.UrlRewrite.Entities.Conditions;
using Hi.UrlRewrite.Entities.Match;
using System;
using System.Collections.Generic;

namespace Hi.UrlRewrite.Entities.Rules
{
    [Serializable]
    public class InboundRule : IBaseRule, IConditionList
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; }
        public string SiteNameRestriction { get; set; }

        public IBaseAction Action { get; set; }

        public bool Enabled { get; set; }

        public LogicalGrouping? ConditionLogicalGrouping { get; set; }
        public List<Condition> Conditions { get; set; }

        public MatchType? MatchType { get; set; }
        public Using? Using { get; set; }
        public string Pattern { get; set; }
        public bool IgnoreCase { get; set; }

        public InboundRule()
        {
            Conditions = new List<Condition>();
        }
    }
}
