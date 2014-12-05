using System.Threading;
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

        public ProcessRequestResult ProcessRequestUrl(Uri requestUri, List<InboundRule> inboundRules)
        {
            if (inboundRules == null)
            {
                throw new ArgumentNullException("inboundRules");
            }

            var originalUri = requestUri;

            Log.Debug(string.Format("UrlRewrite - Processing url: {0}", originalUri), this);

            var hasAtLeastOneRewrite = false;

            var ruleResult = new RuleResult
            {
                RewrittenUri = originalUri
            };

            var processedResults = new List<RuleResult>();

            foreach (var inboundRule in inboundRules)
            {
                ruleResult = ProcessInboundRule(ruleResult.RewrittenUri, inboundRule);
                processedResults.Add(ruleResult);

                if (ruleResult.RewrittenUri == null) 
                    continue;

                hasAtLeastOneRewrite = true;

                if (ruleResult.StopProcessing)
                {
                    break;
                }
            }

            // reset the orignal uri
            ruleResult.OriginalUri = originalUri;

            Log.Debug(string.Format("UrlRewrite - Processed originalUrl: {0} redirectedUrl: {1}", ruleResult.OriginalUri, ruleResult.RewrittenUri), this);

            var finalResult = new ProcessRequestResult(ruleResult, hasAtLeastOneRewrite, processedResults);

            return finalResult;
        }

        public void ExecuteRedirect(HttpResponseBase httpResponse, ProcessRequestResult ruleResult)
        {
            try
            {
                httpResponse.Clear();
                httpResponse.RedirectLocation = ruleResult.RewrittenUri.ToString();
                httpResponse.StatusCode = ruleResult.StatusCode ?? (int)HttpStatusCode.MovedPermanently;

                if (ruleResult.HttpCacheability.HasValue)
                {
                    httpResponse.Cache.SetCacheability(ruleResult.HttpCacheability.Value);
                }

                httpResponse.End();
            }
            catch (ThreadAbortException)
            {
                // swallow this exception because we may have called Response.End
            }
        }

        private RuleResult ProcessInboundRule(Uri originalUri, InboundRule inboundRule)
        {
            Log.Debug(string.Format("UrlRewrite - Processing inbound rule - requestUri: {0} inboundRule: {1}", originalUri, inboundRule.Name), this);

            var ruleResult = new RuleResult
            {
                OriginalUri = originalUri,
                RewrittenUri = originalUri
            };

            switch (inboundRule.Using)
            {
                case Using.ExactMatch:
                case Using.RegularExpressions:

                    ruleResult = ProcessRegularExpressionInboundRule(ruleResult.OriginalUri, inboundRule);

                    break;
                case Using.Wildcards:
                    //TODO: Implement Wildcards
                    throw new NotImplementedException("Using Wildcards has not been implemented");
                    break;
                  
                default:
                    break;
            }

            Log.Debug(string.Format("UrlRewrite - Processing inbound rule - requestUri: {0} inboundRule: {1} rewrittenUrl: {2}", ruleResult.OriginalUri, inboundRule.Name, ruleResult.RewrittenUri), this);

            ruleResult.ItemId = inboundRule.ItemId;

            return ruleResult;
        }

        private RuleResult ProcessRegularExpressionInboundRule(Uri originalUri, InboundRule inboundRule)
        {

            var ruleResult = new RuleResult
            {
                OriginalUri = originalUri,
                RewrittenUri = originalUri
            };

            string rewrittenUrl = null;

            var host = originalUri.Host;

            var absolutePath = originalUri.AbsolutePath;
            var uriPath = absolutePath.Substring(1); // remove starting "/"

            var escapedAbsolutePath = HttpUtility.UrlDecode(absolutePath);
            var escapedUriPath = (escapedAbsolutePath ?? string.Empty).Substring(1); // remove starting "/"

            var matchesThePattern = inboundRule.RequestedUrl.HasValue &&
                                    inboundRule.RequestedUrl.Value == RequestedUrl.MatchesThePattern;

            if (!matchesThePattern)
            {
                throw new NotImplementedException("Have not yet implemented 'Does Not Match the Pattern' because of possible redirect loops");
            }

            var pattern = inboundRule.Pattern;

            if (inboundRule.Using.HasValue && inboundRule.Using.Value == Using.ExactMatch)
            {
                pattern = "^" + pattern + "$";
            }

            var inboundRuleRegex = new Regex(pattern, inboundRule.IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);

            var inboundRuleMatch = inboundRuleRegex.Match(uriPath);
            var isInboundRuleMatch = matchesThePattern ? inboundRuleMatch.Success : !inboundRuleMatch.Success;

            Log.Debug(string.Format("UrlRewrite - Regex - Pattern: '{0}' Input: '{1}' Success: {2}", pattern, uriPath, isInboundRuleMatch), this);

            if (!isInboundRuleMatch && !uriPath.Equals(escapedUriPath, StringComparison.InvariantCultureIgnoreCase))
            {
                inboundRuleMatch = inboundRuleRegex.Match(escapedUriPath);
                isInboundRuleMatch = matchesThePattern ? inboundRuleMatch.Success : !inboundRuleMatch.Success;

                Log.Debug(string.Format("UrlRewrite - Regex - Pattern: '{0}' Input: '{1}' Success: {2}", pattern, escapedUriPath, isInboundRuleMatch), this);
            }

            var conditionLogicalGrouping = inboundRule.LogicalGrouping.HasValue ? inboundRule.LogicalGrouping.Value : LogicalGrouping.MatchAll;
            var query = originalUri.Query;
            var https = originalUri.Scheme.Equals("https", StringComparison.InvariantCultureIgnoreCase) ? "on" : "off"; //HttpContext.Current.Request.ServerVariables["HTTPS"];

            if (isInboundRuleMatch && inboundRule.Conditions != null && inboundRule.Conditions.Count > 0)
            {
                var conditionMatches = false;
                if (conditionLogicalGrouping == LogicalGrouping.MatchAll)
                {
                    conditionMatches = inboundRule.Conditions.All(condition => ConditionMatch(host, query, https, condition));
                }
                else
                {
                    conditionMatches = inboundRule.Conditions.Any(condition => ConditionMatch(host, query, https, condition));
                }

                isInboundRuleMatch = conditionMatches;
            }

            if (isInboundRuleMatch)
            {

                Log.Debug(string.Format("UrlRewrite - INBOUND RULE MATCH - requestUri: {0} inboundRule: {1}", originalUri, inboundRule.Name), this);

                // TODO: Need to implement Rewrite, None, Custom Response and Abort Request

                // process the action if it is a RedirectAction
                if (inboundRule.Action is RedirectAction)
                {
                    var redirectAction = inboundRule.Action as RedirectAction;

                    var rewriteUrl = redirectAction.RewriteUrl;
                    var rewriteItemId = redirectAction.RewriteItemId;
                    var rewriteItmAnchor = redirectAction.RewriteItemAnchor;

                    if (rewriteItemId.HasValue)
                    {
                        var db = Sitecore.Context.Database; // Database.GetDatabase(Configuration.Database);
                        if (db != null)
                        {
                            var rewriteItem = db.GetItem(new ID(rewriteItemId.Value));

                            if (rewriteItem != null)
                            {
                                var urlOptions = new UrlOptions
                                {
                                    AlwaysIncludeServerUrl = true, 
                                    SiteResolving = true
                                };

                                rewriteUrl = LinkManager.GetItemUrl(rewriteItem, urlOptions);

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
                    ruleResult.StatusCode = redirectType.HasValue ? (int) redirectType : (int) HttpStatusCode.MovedPermanently;

                    if (redirectAction.AppendQueryString)
                    {
                        rewriteUrl += query;
                    }

                    ruleResult.RewrittenUri = new Uri(rewriteUrl);
                    ruleResult.StopProcessing = redirectAction.StopProcessingOfSubsequentRules;
                    ruleResult.HttpCacheability = redirectAction.HttpCacheability;
                }
                else
                {
                    throw new NotImplementedException("Redirect Action is currently the only supported type of redirect");
                }
            }

            return ruleResult;
        }

        private bool ConditionMatch(string host, string query, string https, Condition condition)
        {

            // TODO : I have only implemented "MatchesThePattern" - need to implement the other types

            var conditionRegex = new Regex(condition.Pattern, condition.IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);

            bool isMatch = false;

            if (condition.CheckIfInputString.HasValue)
            {
                switch (condition.CheckIfInputString.Value)
                {
                    case CheckIfInputStringType.MatchesThePattern:
                    case CheckIfInputStringType.DoesNotMatchThePattern:
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

                        if (condition.CheckIfInputString.Value == CheckIfInputStringType.DoesNotMatchThePattern)
                        {
                            isMatch = !isMatch;
                        }

                        break;
                    default:
                        throw new NotImplementedException("Only 'Matches the Pattern' and 'Does Not Match the Pattern' have been implemented.");
                        break;
                }

            }

            return isMatch;
        }

    }
}
