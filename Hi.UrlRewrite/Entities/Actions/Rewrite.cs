using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hi.UrlRewrite.Entities.Actions.Base;

namespace Hi.UrlRewrite.Entities.Actions
{
    [Serializable]
    public class Rewrite : IBaseRewrite
    {
        public bool StopProcessingOfSubsequentRules { get; set; }
        public string Name { get; set; }
        public bool AppendQueryString { get; set; }
        public string RewriteUrl { get; set; }
        public Guid? RewriteItemId { get; set; }
        public string RewriteItemQueryString { get; set; }
        public string RewriteItemAnchor { get; set; }
    }
}
