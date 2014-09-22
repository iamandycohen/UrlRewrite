using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Hi.UrlRewrite.Entities
{
    public class BaseRedirectAction : BaseStopProcessingAction, IBaseCache
    {
        public string RewriteUrl { get; set; }
        public Guid? RewriteItemId { get; set; }
        public string RewriteItemAnchor { get; set; }
        public bool AppendQueryString { get; set; }
        public HttpCacheability? HttpCacheability { get; set; }
    }
}
