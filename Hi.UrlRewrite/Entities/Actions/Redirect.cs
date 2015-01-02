using Hi.UrlRewrite.Entities.Actions.Base;
using System;
using System.Web;

namespace Hi.UrlRewrite.Entities.Actions
{
    [Serializable]
    public class Redirect : IBaseRedirect
    {
        public string Name { get; set; }
        public string RewriteUrl { get; set; }
        public Guid? RewriteItemId { get; set; }
        public string RewriteItemAnchor { get; set; }
        public bool AppendQueryString { get; set; }

        public HttpCacheability? HttpCacheability { get; set; }
        public bool StopProcessingOfSubsequentRules { get; set; }
        public RedirectStatusCode? StatusCode { get; set; }

    }
}
