using Hi.UrlRewrite.Entities.Actions.Base;
using Hi.UrlRewrite.Entities.Conditions;
using Hi.UrlRewrite.Entities.Match;
using System;
using System.Collections.Generic;

namespace Hi.UrlRewrite.Entities.Rules
{
    [Serializable]
    public class OutboundRule : IBaseRule, IPrecondition, IOutboundMatch, IConditionList, IBaseEnabled
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public IEnumerable<Condition> Conditions { get; set; }
        public LogicalGrouping? ConditionLogicalGrouping { get; set; }
        public MatchType? MatchType { get; set; }
        public Using? Using { get; set; }
        public string Pattern { get; set; }
        public bool IgnoreCase { get; set; }

        public Precondition Precondition { get; set; }

        public ScopeType MatchingScopeType { get; set; }
        public IBaseAction Action { get; set; }

        public OutboundRule()
        {
            MatchingScopeType = ScopeType.Response;
            Conditions = new List<Condition>();
        }

        public IBaseMatchScope OutboundMatchScope { get; set; }
    }
}