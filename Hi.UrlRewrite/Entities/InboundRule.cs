using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.UrlRewrite.Entities
{
    public class InboundRule
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; }
        public RequestedUrl? RequestedUrl { get; set; }
        public string Pattern { get; set; }
        public bool IgnoreCase { get; set; }
        public BaseAction Action { get; set; }
        public LogicalGrouping? ConditionLogicalGrouping { get; set; }
        public Using? Using { get; set; }
        public List<Condition> Conditions { get; set; }
        public bool Enabled { get; set; }
        public string SiteNameRestriction { get; set; }
    }
}
