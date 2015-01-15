using Hi.UrlRewrite.Entities.Reporting;
using Hi.UrlRewrite.Entities.Rules;
using Hi.UrlRewrite.Processing;
using Hi.UrlRewrite.Templates;
using Hi.UrlRewrite.Templates.Folders;
using Hi.UrlRewrite.Templates.Inbound;
using Lucene.Net.Search;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hi.UrlRewrite.Reporting
{
    public class ReportingManager
    {
        public IEnumerable<RewriteReport> GetRewriteReports()
        {
            IEnumerable<RewriteReport> rewriteReports = new List<RewriteReport>();

            var index = SearchManager.GetIndex("UrlRewriteReporting");
            if (index == null) return rewriteReports;

            var rulesEngine = new RulesEngine();

            using (var indexSearchContext = index.CreateSearchContext())
            {
                //var query = new FieldQuery(BuiltinFields.Template, ShortID.Encode(RewriteReportItem.TemplateId));
                var query = new MatchAllDocsQuery();
                var searchHits = indexSearchContext.Search(query, int.MaxValue);

                rewriteReports = searchHits.FetchResults(0, int.MaxValue)
                    .Select(searchResult => searchResult.GetObject<Item>())
                    .Select(item => GetRewriteReport(item, rulesEngine))
                    .Where(report => report != null);
            }

            return rewriteReports;
        }

        private RewriteReport GetRewriteReport(Item rewriteReportItem, RulesEngine rulesEngine)
        {
            var rewriteReport = new RewriteReportItem(rewriteReportItem);
            var ruleItem = rewriteReportItem.Database.GetItem(rewriteReport.Rule.Value);
            var redirectFolderItem = ruleItem.Axes.GetAncestors()
                .FirstOrDefault(a => a.TemplateID.Equals(new ID(RedirectFolderItem.TemplateId)));

            InboundRule rule;

            if (ruleItem.TemplateID.Equals(new ID(SimpleRedirectItem.TemplateId)))
            {
                rule = rulesEngine.CreateInboundRuleFromSimpleRedirectItem(ruleItem, redirectFolderItem);
            }
            else if (ruleItem.TemplateID.Equals(new ID(InboundRuleItem.TemplateId)))
            {
                rule = rulesEngine.CreateInboundRuleFromInboundRuleItem(ruleItem, redirectFolderItem);
            }
            else
            {
                return null;
            }

            var report = new RewriteReport
            {
                DatabaseName = rewriteReport.DatabaseName.Value,
                OriginalUrl = rewriteReport.OriginalUrl.Value,
                RewrittenUrl = rewriteReport.RewrittenUrl.Value,
                RewriteDate = rewriteReport.RewriteDate.DateTime,
                Rule = rule
            };

            return report;
        }


    }
}