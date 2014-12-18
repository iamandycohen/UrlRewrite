using Hi.UrlRewrite.Entities.Rules;
using Sitecore;
using Sitecore.Caching;
using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hi.UrlRewrite.Caching
{
    public class RulesCache : CustomCache
    {
        private Database _db;

        public RulesCache(Database db) : 
            base(string.Format("Hi.UrlRewrite[{0}]", db.Name), StringUtil.ParseSizeString("10MB"))
        {
            _db = db;
        }

        //public List<InboundRule> GetInboundRules()
        //{
            
        //}

    }
}