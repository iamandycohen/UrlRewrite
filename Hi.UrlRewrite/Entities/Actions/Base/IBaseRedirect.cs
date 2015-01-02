using System;
using System.Web;

namespace Hi.UrlRewrite.Entities.Actions.Base
{
    public interface IBaseRedirect : IBaseRewriteUrl, IBaseAppendQueryString, IBaseCache, IBaseStopProcessing, IBaseStatusCode
    {
        string Name { get; set; }
        string RewriteUrl { get; set; }
        Guid? RewriteItemId { get; set; }
        string RewriteItemAnchor { get; set; }
        bool AppendQueryString { get; set; }

        HttpCacheability? HttpCacheability { get; set; }
        bool StopProcessingOfSubsequentRules { get; set; }
        RedirectStatusCode? StatusCode { get; set; }
    }
}
