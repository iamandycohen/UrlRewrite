using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Hi.UrlRewrite.Entities.Rules;
using Sitecore;
using Sitecore.Caching;
using Sitecore.Data;
using System.Collections.Generic;
using System.Linq;

namespace Hi.UrlRewrite.Caching
{
    public class RulesCache : CustomCache
    {
        private Database _db;
        private const string inboundRulesKey = "InboundRules";
        private const string outboundRulesKey = "OutboundRules";

        public RulesCache(Database db) :
            base(string.Format("Hi.UrlRewrite[{0}]", db.Name), StringUtil.ParseSizeString(Configuration.CacheSize))
        {
            _db = db;
        }

        public List<InboundRule> GetInboundRules()
        {
            return GetRules<InboundRule>(inboundRulesKey);
        }

        public void SetInboundRules(IEnumerable<InboundRule> inboundRules)
        {
            SetRules(inboundRules, inboundRulesKey);
        }

        public List<OutboundRule> GetOutboundRules()
        {
            return GetRules<OutboundRule>(outboundRulesKey);
        }

        public void SetOutboundRules(IEnumerable<OutboundRule> outboundRules)
        {
            SetRules(outboundRules, outboundRulesKey);
        }

        public List<T> GetRules<T>(string key) where T : IBaseRule
        {
            List<T> returnRules = null;
            var rules = InnerCache.GetValue(key) as IEnumerable<T>;
            if (rules != null)
            {
                returnRules = rules.ToList();
            }

            return returnRules;
        }

        public void SetRules<T>(IEnumerable<T> rules, string key) where T : IBaseRule
        {

            long size;

            using (var memoryStream = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, rules.ToList());
                size = memoryStream.Length;
            }

            InnerCache.Add(key, rules, size);
        }

        public void ClearInboundRules()
        {
            RemoveKeysContaining(inboundRulesKey);
        }

        public void ClearOutboundRules()
        {
            RemoveKeysContaining(outboundRulesKey);
        }

    }
}