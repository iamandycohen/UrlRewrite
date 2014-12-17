using System;
using System.Web;

namespace Hi.UrlRewrite.Entities.Actions
{
    public class BaseRedirectAction : IBaseRewrite
    {
        public string RewriteUrl { get; set; }
        public Guid? RewriteItemId { get; set; }
        public string RewriteItemAnchor { get; set; }
        public bool AppendQueryString { get; set; }

        public HttpCacheability? HttpCacheability { get; set; }
        public bool StopProcessingOfSubsequentRules { get; set; }
        public string Name { get; set; }
        public RedirectActionStatusCode? StatusCode { get; set; }
    }
}
