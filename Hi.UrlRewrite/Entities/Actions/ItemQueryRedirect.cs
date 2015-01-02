using Hi.UrlRewrite.Entities.Actions.Base;
using System;
using System.Web;

namespace Hi.UrlRewrite.Entities.Actions
{
    [Serializable]
    public class ItemQueryRedirect : IBaseAppendQueryString, IBaseCache, IBaseStopProcessing, IBaseStatusCode
    {
        public string ItemQuery { get; set; }
        public HttpCacheability? HttpCacheability { get; set; }
        public RedirectStatusCode? StatusCode { get; set; }
        public bool StopProcessingOfSubsequentRules { get; set; }
        public string Name { get; set; }
        public bool AppendQueryString { get; set; }
    }
}