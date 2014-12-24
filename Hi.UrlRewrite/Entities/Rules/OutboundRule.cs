using System;
using System.Collections.Generic;
using Hi.UrlRewrite.Entities.Actions;
using Hi.UrlRewrite.Entities.Conditions;
using Hi.UrlRewrite.Entities.Match;

namespace Hi.UrlRewrite.Entities.Rules
{
    [Serializable]
    public class OutboundRule : IBaseRule, IPrecondition, IOutboundMatch
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public List<Condition> Conditions { get; set; }
        public LogicalGrouping? ConditionLogicalGrouping { get; set; }
        public MatchType? MatchType { get; set; }
        public Using? Using { get; set; }
        public string Pattern { get; set; }
        public bool IgnoreCase { get; set; }
        public Precondition Precondition { get; set; }
        public ScopeType? MatchingScope { get; set; }
        public string ScopeValue { get; set; }
        public IBaseAction Action { get; set; }
    }
}