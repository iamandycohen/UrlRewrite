using System;
using System.Collections.Generic;
using Hi.UrlRewrite.Entities.Actions;
using Hi.UrlRewrite.Entities.Conditions;

namespace Hi.UrlRewrite.Entities.Rules
{
    public class InboundRule
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; }
        public RequestedUrl? RequestedUrl { get; set; }
        public string Pattern { get; set; }
        public bool IgnoreCase { get; set; }
        public IBaseAction Action { get; set; }
        public LogicalGrouping? ConditionLogicalGrouping { get; set; }
        public Using? Using { get; set; }
        public List<Condition> Conditions { get; set; }
        public bool Enabled { get; set; }
        public string SiteNameRestriction { get; set; }

        public InboundRule()
        {
            Conditions = new List<Condition>();
        }
    }
}
