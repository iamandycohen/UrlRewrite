using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hi.UrlRewrite.Caching
{
    public static class RulesCacheManager
    {
        private static Dictionary<string, RulesCache> Caches = new Dictionary<string, RulesCache>();

        public static RulesCache GetCache(Database db)
        {
            if (Caches.ContainsKey(db.Name))
            {
                return Caches[db.Name];
            }
            else
            {
                var cache = new RulesCache(db);
                Caches.Add(db.Name, cache);

                return cache;
            }
        }

    }
}