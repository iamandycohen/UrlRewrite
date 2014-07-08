using Hi.UrlRewrite.Entities;
using Hi.UrlRewrite.Templates;
using Hi.UrlRewrite.Templates.Conditions;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Hi.UrlRewrite
{
    public class UrlRewriter
    {
        private static object thisType = typeof(UrlRewriter);

        public static void ProcessRequestUrl(List<InboundRule> inboundRules, Uri requestUri)
        {

            string originalUrl = requestUri.ToString();
            if (inboundRules == null)
            {
                throw new ArgumentNullException("inboundRules");
            }

            Log.Debug(string.Format("UrlRewrite - Processing {0}", requestUri), thisType);

            bool stopProcessing = false;
            bool hasAtLeastOneRewrite = false;
            string rewrittenUrl = null;
            int? statusCode = null;

            using (new SecurityDisabler())
            {
                foreach (var inboundRule in inboundRules)
                {
                    rewrittenUrl = ProcessInboundRule(requestUri, inboundRule, out statusCode, out stopProcessing);

                    if (rewrittenUrl != null)
                    {
                        hasAtLeastOneRewrite = true;
                        requestUri = new Uri(rewrittenUrl);

                        if (stopProcessing)
                        {
                            break;
                        }
                    }
                }
            }

            var redirectedUrl = requestUri.ToString();
            Log.Debug(string.Format("UrlRewrite - Processed originalUrl: {0} redirectedUrl: {1}", originalUrl, redirectedUrl), thisType);

            if (hasAtLeastOneRewrite && !string.IsNullOrEmpty(redirectedUrl))
            {
                ExecuteRedirect(originalUrl, redirectedUrl, statusCode ?? (int)HttpStatusCode.MovedPermanently);
            }
        }

        private static string ProcessInboundRule(Uri requestUri, InboundRule inboundRule, out int? statusCode, out bool stopProcessing)
        {
            Log.Debug(string.Format("UrlRewrite - Processing inbound rule - requestUri: {0} inboundRule: {1}", requestUri, inboundRule.Name), thisType);

            statusCode = null;
            stopProcessing = false;
            string rewrittenUrl = null;

            //TODO: Right now we are only processing regular expression rules... need to implement the others
            switch (inboundRule.Using)
            {
                case Using.RegularExpressions:
                    rewrittenUrl = ProcessRegularExpressionInboundRule(requestUri, inboundRule, out statusCode, out stopProcessing);
                    break;
                case Using.Wildcards:
                    break;
                case Using.ExactMatch:
                    break;
                default:
                    break;
            }

            Log.Debug(string.Format("UrlRewrite - Processing inbound rule - requestUri: {0} inboundRule: {1} rewrittenUrl: {2}", requestUri, inboundRule.Name, rewrittenUrl), thisType);

            return rewrittenUrl;
        }

        private static string ProcessRegularExpressionInboundRule(Uri requestUri, InboundRule inboundRule, out int? statusCode, out bool stopProcessing)
        {
            statusCode = null;
            stopProcessing = false;

            string rewrittenUrl = null;

            var absolutePath = requestUri.AbsolutePath;
            var uriPath = absolutePath.Substring(1); // remove starting "/"
            var host = requestUri.Host;
            var inboundRuleRegex = new Regex(inboundRule.Pattern, inboundRule.IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
            var inboundRuleMatch = inboundRuleRegex.Match(uriPath);
            var isInboundRuleMatch = inboundRuleMatch.Success; // inboundRuleRegex.IsMatch(uriPath);
            var conditionLogicalGrouping = inboundRule.LogicalGrouping.HasValue ? inboundRule.LogicalGrouping.Value : LogicalGrouping.MatchAll;
            var query = requestUri.Query;
            var https = requestUri.Scheme.Equals("https", StringComparison.InvariantCultureIgnoreCase) ? "on" : "off"; //HttpContext.Current.Request.ServerVariables["HTTPS"];

            if (isInboundRuleMatch && inboundRule.Conditions.Count > 0)
            {
                var conditionMatches = false;
                if (conditionLogicalGrouping == LogicalGrouping.MatchAll)
                {
                    conditionMatches = inboundRule.Conditions.All(condition =>
                    {
                        return ConditionMatch(host, query, https, condition);
                    });
                }
                else
                {
                    conditionMatches = inboundRule.Conditions.Any(condition =>
                    {
                        return ConditionMatch(host, query, https, condition);
                    });
                }

                isInboundRuleMatch = conditionMatches;
            }

            if (isInboundRuleMatch)
            {

                Log.Debug(string.Format("UrlRewrite - INBOUND RULE MATCH - requestUri: {0} inboundRule: {1}", requestUri, inboundRule.Name), thisType);

                //TODO: create other Actions
                // process the action if it is a RedirectAction
                if (inboundRule.Action is RedirectAction)
                {
                    var redirectAction = inboundRule.Action as RedirectAction;

                    var rewriteUrl = redirectAction.RewriteUrl;
                    var rewriteItemId = redirectAction.RewriteItemId;
                    var rewriteItmAnchor = redirectAction.RewriteItemAnchor;

                    if (rewriteItemId.HasValue)
                    {
                        var db = Database.GetDatabase(Configuration.Database);
                        if (db != null)
                        {
                            var rewriteItem = db.GetItem(new ID(rewriteItemId.Value));
                            if (rewriteItem != null)
                            {
                                rewriteUrl = string.Format("http://{{HTTP_HOST}}{0}", LinkManager.GetItemUrl(rewriteItem));
                                if (!string.IsNullOrEmpty(rewriteItmAnchor))
                                {
                                    rewriteUrl += string.Format("#{0}", rewriteItmAnchor);
                                }
                            }
                        }
                    }

                    // process token replacements

                    // replace host
                    rewriteUrl = rewriteUrl.Replace("{HTTP_HOST}", host);

                    // process capture groups
                    var ruleCaptureGroupRegex = new Regex(@"({R:(\d+)})", RegexOptions.None);

                    foreach (Match ruleCaptureGroupMatch in ruleCaptureGroupRegex.Matches(rewriteUrl))
                    {
                        var num = ruleCaptureGroupMatch.Groups[2];
                        var groupIndex = System.Convert.ToInt32(num.Value);
                        var group = inboundRuleMatch.Groups[groupIndex];
                        var matchText = ruleCaptureGroupMatch.ToString();

                        rewriteUrl = rewriteUrl.Replace(matchText, group.Value);
                    }

                    var redirectType = redirectAction.RedirectType;
                    // get the status code
                    statusCode = redirectType.HasValue ? (int)redirectType : (int)HttpStatusCode.MovedPermanently;

                    if (redirectAction.AppendQueryString)
                    {
                        rewriteUrl += query;
                    }

                    rewrittenUrl = rewriteUrl;
                    stopProcessing = redirectAction.StopProcessingOfSubsequentRules;
                }
            }

            return rewrittenUrl;
        }

        private static bool ConditionMatch(string host, string query, string https, Condition condition)
        {
            var conditionRegex = new Regex(condition.Pattern, condition.IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
            var matchesThePattern = condition.CheckIfInputString.HasValue ? condition.CheckIfInputString.Value == CheckIfInputStringType.MatchesThePattern : false;

            bool isMatch = false;
            switch (condition.ConditionInput)
            {
                case Hi.UrlRewrite.Entities.ConditionInputType.HTTP_HOST:
                    isMatch = conditionRegex.IsMatch(host);
                    break;
                case Hi.UrlRewrite.Entities.ConditionInputType.QUERY_STRING:
                    isMatch = conditionRegex.IsMatch(query);
                    break;
                case Hi.UrlRewrite.Entities.ConditionInputType.HTTPS:
                    isMatch = conditionRegex.IsMatch(https);
                    break;
                default:
                    break;
            }

            return matchesThePattern ? isMatch : !isMatch;
        }

        private static void ExecuteRedirect(string originalUrl, string rewriteUrl, int statusCode)
        {
            var context = HttpContext.Current;
            var response = context.Response;

            Log.Info(string.Format("UrlRewrite - Redirecting {0} to {1} [{2}]", originalUrl, rewriteUrl, statusCode), thisType);
            response.Clear();
            response.RedirectLocation = rewriteUrl;
            response.StatusCode = statusCode;
            response.End();
        }
    }
}
