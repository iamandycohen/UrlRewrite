using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.UrlRewrite.Entities
{
    public class BaseRedirectAction : BaseStopProcessingAction
    {
        public string RewriteUrl { get; set; }
        public Guid? RewriteItemId { get; set; }
        public string RewriteItemAnchor { get; set; }
        public bool AppendQueryString { get; set; }
    }
}
