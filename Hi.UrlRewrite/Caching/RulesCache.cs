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
            List<InboundRule> returnRules = null;
            var rules = GetObject(inboundRulesKey) as IEnumerable<InboundRule>;
            if (rules != null)
            {
                returnRules = rules.ToList();
            }

            return returnRules;
        }

        public void SetInboundRules(IEnumerable<InboundRule> inboundRules)
        {
            long size;

            using (var memoryStream = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, inboundRules.ToList());
                size = memoryStream.Length;
            }

            SetObject(inboundRulesKey, inboundRules, size);
        }

        public List<OutboundRule> GetOutboundRules()
        {
            List<OutboundRule> returnRules = null;
            var rules = GetObject(inboundRulesKey) as IEnumerable<OutboundRule>;
            if (rules != null)
            {
                returnRules = rules.ToList();
            }

            return returnRules;
        }

        public void SetOutboundRules(IEnumerable<OutboundRule> outboundRules)
        {
            long size;

            using (var memoryStream = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, outboundRules.ToList());
                size = memoryStream.Length;
            }

            SetObject(outboundRulesKey, outboundRules, size);
        }

    }
}