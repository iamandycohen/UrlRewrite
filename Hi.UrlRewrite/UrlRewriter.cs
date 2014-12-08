using System.Threading;
using Hi.UrlRewrite.Entities;
using Hi.UrlRewrite.Templates;
using Hi.UrlRewrite.Templates.Conditions;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Exceptions;
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

            var matchedAtLeastOneRule = false;

            var ruleResult = new RuleResult
            {
                RewrittenUri = originalUri
            };

            var processedResults = new List<RuleResult>();

            foreach (var inboundRule in inboundRules)
            {
                ruleResult = ProcessInboundRule(ruleResult.RewrittenUri, inboundRule);
                processedResults.Add(ruleResult);

                if (!ruleResult.RuleMatched) 
                    continue;

                matchedAtLeastOneRule = true;

                if (ruleResult.RuleMatched && ruleResult.StopProcessing)
                {
                    break;
                }
            }

            // we are done with the looping, reset the orignal uri to the real original uri
            ruleResult.OriginalUri = originalUri;

            Log.Debug(string.Format("UrlRewrite - Processed originalUrl: {0} redirectedUrl: {1}", ruleResult.OriginalUri, ruleResult.RewrittenUri), this);

            var finalResult = new ProcessRequestResult(ruleResult, matchedAtLeastOneRule, processedResults);

            return finalResult;
        }

        public void ExecuteResult(HttpResponseBase httpResponse, ProcessRequestResult ruleResult)
        {
            try
            {
                httpResponse.Clear();

                if (!ruleResult.Abort)
                {
                    httpResponse.RedirectLocation = ruleResult.RewrittenUri.ToString();
                    httpResponse.StatusCode = ruleResult.StatusCode ?? (int) HttpStatusCode.MovedPermanently;

                    if (ruleResult.HttpCacheability.HasValue)
                    {
                        httpResponse.Cache.SetCacheability(ruleResult.HttpCacheability.Value);
                    }
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

            var conditionLogicalGrouping = inboundRule.ConditionLogicalGrouping.HasValue ? inboundRule.ConditionLogicalGrouping.Value : LogicalGrouping.MatchAll;

            var host = originalUri.Host;
            var query = originalUri.Query;
            var https = originalUri.Scheme.Equals("https", StringComparison.InvariantCultureIgnoreCase) ? "on" : "off"; //

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

                ruleResult.RuleMatched = true;

                Log.Debug(string.Format("UrlRewrite - INBOUND RULE MATCH - requestUri: {0} inboundRule: {1}", originalUri, inboundRule.Name), this);

                // TODO: Need to implement Rewrite, None, Custom Response

                if (inboundRule.Action == null)
                {
                    Log.Warn(string.Format("UrlRewrite - Inbound Rule has no Action set - inboundRule: {0} inboundRule ItemId: {1}", inboundRule.Name, inboundRule.ItemId), this);

                    //throw new ItemNullException("Inbound Rule has no Action set.");
                } 
                else if (inboundRule.Action is RedirectAction) // process the action if it is a RedirectAction
                {
                    ProcessRedirectAction(inboundRule, host, inboundRuleMatch, ruleResult, query);
                }
                else if (inboundRule.Action is AbortRequestAction)
                {
                    ProcessAbortRequestAction(inboundRule, ruleResult);
                }
                else
                {
                    throw new NotImplementedException("Redirect Action is currently the only supported type of redirect");
                }
            }

            return ruleResult;
        }

        private static void ProcessAbortRequestAction(InboundRule inboundRule, RuleResult ruleResult)
        {
            var abortRequestAction = inboundRule.Action as AbortRequestAction;

            ruleResult.StopProcessing = true;
        }

        private static void ProcessRedirectAction(InboundRule inboundRule, string host, Match inboundRuleMatch,
            RuleResult ruleResult, string query)
        {
            var redirectAction = inboundRule.Action as RedirectAction;

            var rewriteUrl = redirectAction.RewriteUrl;
            var rewriteItemId = redirectAction.RewriteItemId;
            var rewriteItemAnchor = redirectAction.RewriteItemAnchor;

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

                        if (!string.IsNullOrEmpty(rewriteItemAnchor))
                        {
                            rewriteUrl += string.Format("#{0}", rewriteItemAnchor);
                        }
                    }
                }
            }

            // process token replacements

            // replace host
            rewriteUrl = rewriteUrl.Replace("{HTTP_HOST}", host);

            if (redirectAction.AppendQueryString)
            {
                rewriteUrl += query;
            }

            // process capture groups
            var ruleCaptureGroupRegex = new Regex(@"({R:(\d+)})", RegexOptions.None);

            foreach (Match ruleCaptureGroupMatch in ruleCaptureGroupRegex.Matches(rewriteUrl))
            {
                var num = ruleCaptureGroupMatch.Groups[2];
                var groupIndex = System.Convert.ToInt32(num.Value);
                var group = inboundRuleMatch.Groups[groupIndex];
                var matchText = ruleCaptureGroupMatch.ToString();

                rewriteUrl = rewriteUrl.Replace(matchText, @group.Value);
            }

            var redirectType = redirectAction.RedirectType;

            // get the status code
            ruleResult.StatusCode = redirectType.HasValue ? (int) redirectType : (int) HttpStatusCode.MovedPermanently;

            ruleResult.RewrittenUri = new Uri(rewriteUrl);
            ruleResult.StopProcessing = redirectAction.StopProcessingOfSubsequentRules;
            ruleResult.HttpCacheability = redirectAction.HttpCacheability;
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
