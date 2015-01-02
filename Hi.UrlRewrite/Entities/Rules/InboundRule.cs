using Hi.UrlRewrite.Entities.Actions.Base;
using Hi.UrlRewrite.Entities.Conditions;
using Hi.UrlRewrite.Entities.Match;
using Hi.UrlRewrite.Entities.ServerVariables;
using System;
using System.Collections.Generic;

namespace Hi.UrlRewrite.Entities.Rules
{
    [Serializable]
    public class InboundRule : IBaseRule, IConditionList, IServerVariableList
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; }
        public string SiteNameRestriction { get; set; }

        public IBaseAction Action { get; set; }

        public bool Enabled { get; set; }

        public LogicalGrouping? ConditionLogicalGrouping { get; set; }
        public IEnumerable<Condition> Conditions { get; set; }

        public MatchType? MatchType { get; set; }
        public Using? Using { get; set; }
        public string Pattern { get; set; }
        public bool IgnoreCase { get; set; }

        public IEnumerable<ServerVariable> ServerVariables { get; set; }

        public InboundRule()
        {
            Conditions = new List<Condition>();
            ServerVariables = new List<ServerVariable>();
        }

    }
}
