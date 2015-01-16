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
    public class ReportingHelper
    {

        public IEnumerable<RewriteReportGroup> GetRewriteReportsGrouped(RulesEngine rulesEngine)
        {
            var rewriteReports = GetRewriteReports();
            var rewriteReportsGrouped = rewriteReports
                .GroupBy(r => new { DatabaseName = r.DatabaseName, RulePath = r.RulePath })
                .Select(group => new RewriteReportGroup
                {
                    Name = string.Format("{0}::{1}", group.Key.DatabaseName, group.Key.RulePath),
                    Count = group.Count(),
                    Rule = GetInboundRule(rulesEngine, group.Key.DatabaseName, group.Key.RulePath),
                    Reports = group.ToList()
                });

            return rewriteReportsGrouped;
        }

        public InboundRule GetInboundRule(RulesEngine rulesEngine, string databaseName, string rewriteReportItemPath)
        {
            var database = Database.GetDatabase(databaseName);
            var rewriteReportItem = database.GetItem(rewriteReportItemPath);
            var redirectFolderItem = rewriteReportItem.Axes.GetAncestors()
                .FirstOrDefault(a => a.TemplateID.Equals(new ID(RedirectFolderItem.TemplateId)));

            InboundRule rule;

            if (rewriteReportItem.TemplateID.Equals(new ID(SimpleRedirectItem.TemplateId)))
            {
                rule = rulesEngine.CreateInboundRuleFromSimpleRedirectItem(rewriteReportItem, redirectFolderItem);
            }
            else if (rewriteReportItem.TemplateID.Equals(new ID(InboundRuleItem.TemplateId)))
            {
                rule = rulesEngine.CreateInboundRuleFromInboundRuleItem(rewriteReportItem, redirectFolderItem);
            }
            else
            {
                return null;
            }

            return rule;
        }

        public IEnumerable<RewriteReport> GetRewriteReports()
        {
            IEnumerable<RewriteReport> rewriteReports = new List<RewriteReport>();

            var index = SearchManager.GetIndex("UrlRewriteReporting");
            if (index == null) return rewriteReports;

            using (var indexSearchContext = index.CreateSearchContext())
            {
                //var query = new FieldQuery(BuiltinFields.Template, ShortID.Encode(RewriteReportItem.TemplateId));
                var query = new MatchAllDocsQuery();
                var searchHits = indexSearchContext.Search(query, int.MaxValue);

                rewriteReports = searchHits.FetchResults(0, int.MaxValue)
                    .Select(searchResult => searchResult.GetObject<Item>())
                    .Select(item => GetRewriteReport(item))
                    .Where(report => report != null);
            }

            return rewriteReports;
        }

        private RewriteReport GetRewriteReport(RewriteReportItem rewriteReport)
        {
            var ruleItem = rewriteReport.InnerItem.Database.GetItem(rewriteReport.Rule.Value);

            var report = new RewriteReport
            {
                DatabaseName = rewriteReport.DatabaseName.Value,
                OriginalUrl = rewriteReport.OriginalUrl.Value,
                RewrittenUrl = rewriteReport.RewrittenUrl.Value,
                RewriteDate = rewriteReport.RewriteDate.DateTime,
                RulePath = rewriteReport.Rule.Value
            };

            return report;
        }


    }
}