using Hi.UrlRewrite.Entities.Reporting;
using Hi.UrlRewrite.Entities.Rules;
using Hi.UrlRewrite.Processing;
using Hi.UrlRewrite.Templates;
using Hi.UrlRewrite.Templates.Folders;
using Hi.UrlRewrite.Templates.Inbound;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Search;
using System.Collections.Generic;
using System.Linq;

namespace Hi.UrlRewrite.Reporting
{
    public class ReportingHelper
    {

        public IEnumerable<RewriteReportGroup> GetRewriteReportsGrouped(Database database)
        {
            var rewriteReports = GetRewriteReports(database);
            var rewriteReportsGrouped = rewriteReports
                .GroupBy(report => new { DatabaseName = report.DatabaseName, RulePath = report.RulePath })
                .Select(group => new RewriteReportGroup
                {
                    Name = string.Format("{0}::{1}", group.Key.DatabaseName, group.Key.RulePath),
                    Rule = GetInboundRule(group.Key.DatabaseName, group.Key.RulePath),
                    Reports = group.ToList()
                })
                .OrderByDescending(group => group.Count);

            return rewriteReportsGrouped;
        }

        public IEnumerable<RewriteReport> GetRewriteReports(Database database)
        {
            IEnumerable<RewriteReport> rewriteReports = new List<RewriteReport>();

            var index = SearchManager.GetIndex("UrlRewriteReporting");
            if (index == null) return rewriteReports;

            var query = CreateQuery(database);
            SearchResultCollection searchResults;

            using (var indexSearchContext = index.CreateSearchContext())
            {
                var searchHits = indexSearchContext.Search(query, int.MaxValue);
                searchResults = searchHits.FetchResults(0, int.MaxValue);
            }

            rewriteReports = searchResults
                .Select(searchResult => searchResult.GetObject<Item>())
                .Select(item => GetRewriteReport(item))
                .Where(report => report != null)
                .OrderBy(r => r.DatabaseName)
                .ThenBy(r => r.RulePath)
                .ThenByDescending(r => r.RewriteDate);

            return rewriteReports;
        }

        private InboundRule GetInboundRule(string reportDatabaseName, string rewriteReportItemPath)
        {
            var reportDatabase = Database.GetDatabase(reportDatabaseName);
            var rulesEngine = new RulesEngine(reportDatabase);
            var rewriteReportItem = reportDatabase.GetItem(rewriteReportItemPath);
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

        private QueryBase CreateQuery(Database database)
        {
            var query = new CombinedQuery();
            var databaseQuery = new FieldQuery(BuiltinFields.Database, database.Name);
            var languageQuery = new FieldQuery(BuiltinFields.Language, Context.Language.Name);
            var templateQuery = new FieldQuery(BuiltinFields.Template, ShortID.Encode(RewriteReportItem.TemplateId));

            query.Add(databaseQuery, QueryOccurance.Must);
            query.Add(languageQuery, QueryOccurance.Must);
            query.Add(templateQuery, QueryOccurance.Must);

            return query;

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