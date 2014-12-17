using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hi.UrlRewrite.Entities.Actions
{
    public class ItemQueryRedirectAction : IBaseRewrite
    {
        public string ItemQuery { get; set; }
        public HttpCacheability? HttpCacheability { get; set; }
        public RedirectActionStatusCode? StatusCode { get; set; }
        public bool StopProcessingOfSubsequentRules { get; set; }
        public string Name { get; set; }
    }
}