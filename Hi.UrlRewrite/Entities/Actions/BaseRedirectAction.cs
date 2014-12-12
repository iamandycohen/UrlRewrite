using System;
using System.Web;

namespace Hi.UrlRewrite.Entities.Actions
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
