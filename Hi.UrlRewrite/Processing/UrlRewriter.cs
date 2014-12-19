using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using Hi.UrlRewrite.Entities.Actions;
using Hi.UrlRewrite.Entities.Conditions;
using Hi.UrlRewrite.Entities.Rules;
using Hi.UrlRewrite.Processing.Results;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.Resources.Media;

namespace Hi.UrlRewrite.Processing
{
    public class UrlRewriter
    {

        public NameValueCollection ServerVariables { get; set; }

        public UrlRewriter()
        {
            ServerVariables = new NameValueCollection();
        }

        public UrlRewriter(NameValueCollection serverVariables)
        {
            ServerVariables = serverVariables;
        }

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
                    ruleResult.StoppedProcessing = true;
                    break;
                }
            }

            Log.Debug(string.Format("UrlRewrite - Processed originalUrl: {0} redirectedUrl: {1}", originalUri, ruleResult.RewrittenUri), this);

            var lastMatchedRuleResult = processedResults.FirstOrDefault(r => r.RuleMatched);

            var finalResult = new ProcessRequestResult(originalUri, lastMatchedRuleResult, matchedAtLeastOneRule, processedResults);

            return finalResult;
        }

        public void ExecuteResult(HttpContextBase httpContext, ProcessRequestResult ruleResult)
        {
            try
            {
                var httpResponse = httpContext.Response;
                httpResponse.Clear();

                if (ruleResult.FinalAction is IBaseRewrite)
                {
                    var redirectAction = ruleResult.FinalAction as IBaseRewrite;
                    int statusCode;

                    if (redirectAction.StatusCode.HasValue)
                    {
                        statusCode = (int)(redirectAction.StatusCode.Value);
                    }
                    else
                    {
                        statusCode = (int)HttpStatusCode.MovedPermanently;
                    }

                    httpResponse.RedirectLocation = ruleResult.RewrittenUri.ToString();
                    httpResponse.StatusCode = statusCode;

                    if (redirectAction.HttpCacheability.HasValue)
                    {
                        httpResponse.Cache.SetCacheability(redirectAction.HttpCacheability.Value);
                    }
                }
                else if (ruleResult.FinalAction is AbortRequestAction)
                {
                    // do nothing
                }
                else if (ruleResult.FinalAction is CustomResponseAction)
                {
                    var customResponse = ruleResult.FinalAction as CustomResponseAction;

                    httpResponse.TrySkipIisCustomErrors = true;

                    httpResponse.StatusCode = customResponse.StatusCode;
                    httpResponse.StatusDescription = customResponse.ErrorDescription;

                    // TODO: Implement Status Reason?
                    //httpResponse.??? = customResponse.Reason;

                    if (customResponse.SubStatusCode.HasValue)
                    {
                        httpResponse.SubStatusCode = customResponse.SubStatusCode.Value;
                    }

                }

                //httpContext.ApplicationInstance.CompleteRequest();
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
                case Using.Wildcards:
                    ruleResult = ProcessRegularExpressionInboundRule(ruleResult.OriginalUri, inboundRule);

                    break;
                //case Using.Wildcards:
                //    //TODO: Implement Wildcards
                //    throw new NotImplementedException("Using Wildcards has not been implemented");
                //    break;
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

            Match inboundRuleMatch,
                lastConditionMatch = null;

            // test rule match
            var isInboundRuleMatch = TestRuleMatches(inboundRule, originalUri, out inboundRuleMatch);
            ConditionMatchResult conditionMatchResult = null;

            // test conditions matches
            if (isInboundRuleMatch && inboundRule.Conditions != null && inboundRule.Conditions.Count > 0)
            {
                conditionMatchResult = TestConditionMatches(inboundRule, originalUri, out lastConditionMatch);
                isInboundRuleMatch = conditionMatchResult.Matched;
            }

            // test site name restrictions
            if (isInboundRuleMatch && !string.IsNullOrEmpty(inboundRule.SiteNameRestriction))
            {
                isInboundRuleMatch = TestSiteNameRestriction(inboundRule);
            }

            if (isInboundRuleMatch && inboundRule.Action != null)
            {
                ruleResult.RuleMatched = true;

                Log.Debug(string.Format("UrlRewrite - INBOUND RULE MATCH - requestUri: {0} inboundRule: {1}", originalUri, inboundRule.Name), this);

                // TODO: Need to implement Rewrite, None

                if (inboundRule.Action is RedirectAction) // process the action if it is a RedirectAction  
                {
                    ProcessRedirectAction(inboundRule, originalUri, inboundRuleMatch, lastConditionMatch, ruleResult);
                }
                else if (inboundRule.Action is ItemQueryRedirectAction)
                {
                    ProcessItemQueryRedirectAction(inboundRule, originalUri, inboundRuleMatch, lastConditionMatch, ruleResult);
                }
                else if (inboundRule.Action is AbortRequestAction || inboundRule.Action is CustomResponseAction)
                {
                    ProcessActionProcessing(ruleResult);
                }
                else
                {
                    throw new NotImplementedException("Redirect Action, Custome Response and Abort Reqeust Action are the only supported type of redirects");
                }

                ruleResult.ResultAction = inboundRule.Action;
                ruleResult.ConditionMatchResult = conditionMatchResult;
            }
            else if (inboundRule.Action == null)
            {
                Log.Warn(string.Format("UrlRewrite - Inbound Rule has no Action set - inboundRule: {0} inboundRule ItemId: {1}", inboundRule.Name, inboundRule.ItemId), this);

                // we are going to skip this because we don't know what to do with it during processing
                ruleResult.RuleMatched = false;
            }

            return ruleResult;
        }

        private bool TestRuleMatches(InboundRule inboundRule, Uri originalUri, out Match inboundRuleMatch)
        {
            var absolutePath = originalUri.AbsolutePath;
            var uriPath = absolutePath.Substring(1); // remove starting "/"

            var escapedAbsolutePath = HttpUtility.UrlDecode(absolutePath);
            var escapedUriPath = (escapedAbsolutePath ?? string.Empty).Substring(1); // remove starting "/"

            // TODO : I have only implemented "MatchesThePattern" - need to implement the other types
            var matchesThePattern = inboundRule.RequestedUrl.HasValue &&
                                    inboundRule.RequestedUrl.Value == RequestedUrl.MatchesThePattern;

            if (!matchesThePattern)
            {
                throw new NotImplementedException(
                    "Have not yet implemented 'Does Not Match the Pattern' because of possible redirect loops");
            }

            var pattern = inboundRule.Pattern;

            if (inboundRule.Using.HasValue && inboundRule.Using.Value == Using.ExactMatch)
            {
                pattern = "^" + pattern + "$";
            }

            var inboundRuleRegex = new Regex(pattern, inboundRule.IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);

            inboundRuleMatch = inboundRuleRegex.Match(uriPath);
            bool isInboundRuleMatch = matchesThePattern ? inboundRuleMatch.Success : !inboundRuleMatch.Success;

            Log.Debug(
                string.Format("UrlRewrite - Regex - Pattern: '{0}' Input: '{1}' Success: {2}", pattern, uriPath,
                    isInboundRuleMatch), this);

            if (!isInboundRuleMatch && !uriPath.Equals(escapedUriPath, StringComparison.InvariantCultureIgnoreCase))
            {
                inboundRuleMatch = inboundRuleRegex.Match(escapedUriPath);
                isInboundRuleMatch = matchesThePattern ? inboundRuleMatch.Success : !inboundRuleMatch.Success;

                Log.Debug(
                    string.Format("UrlRewrite - Regex - Pattern: '{0}' Input: '{1}' Success: {2}", pattern, escapedUriPath,
                        isInboundRuleMatch), this);
            }
            return isInboundRuleMatch;
        }

        private ConditionMatchResult TestConditionMatches(InboundRule inboundRule, Uri originalUri, out Match lastConditionMatch)
        {
            var conditionMatchResult = new ConditionMatchResult();
            bool conditionMatches;
            lastConditionMatch = null;

            var conditionLogicalGrouping = inboundRule.ConditionLogicalGrouping.HasValue
                ? inboundRule.ConditionLogicalGrouping.Value
                : LogicalGrouping.MatchAll;

            conditionMatchResult.LogincalGrouping = conditionLogicalGrouping;

            if (conditionLogicalGrouping == LogicalGrouping.MatchAll)
            {
                foreach (var condition in inboundRule.Conditions)
                {
                    var conditionMatchTuple = ConditionMatch(originalUri, condition, lastConditionMatch);
                    var conditionMatch = conditionMatchTuple.Item1;
                    var conditionInput = conditionMatchTuple.Item2;

                    conditionMatches = conditionMatch.Success;

                    if (condition.CheckIfInputString != null && condition.CheckIfInputString.Value == CheckIfInputString.DoesNotMatchThePattern)
                    {
                        conditionMatches = !conditionMatches;
                    }

                    if (conditionMatches)
                    {
                        lastConditionMatch = conditionMatch;
                        conditionMatchResult.MatchedConditions.Add(new Tuple<Condition, string>(condition, conditionInput));
                        conditionMatchResult.Matched = true;
                    }
                }
            }
            else
            {
                foreach (var condition in inboundRule.Conditions)
                {
                    var conditionMatchTuple = ConditionMatch(originalUri, condition);
                    var conditionMatch = conditionMatchTuple.Item1;
                    var conditionInput = conditionMatchTuple.Item2;

                    conditionMatches = conditionMatch.Success;

                    if (condition.CheckIfInputString != null && condition.CheckIfInputString.Value == CheckIfInputString.DoesNotMatchThePattern)
                    {
                        conditionMatches = !conditionMatches;
                    }

                    if (!conditionMatches) continue;

                    conditionMatchResult.MatchedConditions.Add(new Tuple<Condition, string>(condition, conditionInput));
                    lastConditionMatch = conditionMatch;
                    conditionMatchResult.Matched = true;

                    break;
                }
            }

            return conditionMatchResult;
        }

        private bool TestSiteNameRestriction(InboundRule inboundRule)
        {
            var currentSiteName = Sitecore.Context.Site.Name;
            bool isInboundRuleMatch = false;

            if (currentSiteName != null)
            {
                isInboundRuleMatch = currentSiteName.Equals(inboundRule.SiteNameRestriction,
                    StringComparison.InvariantCultureIgnoreCase);

                if (!isInboundRuleMatch)
                {
                    Log.Debug(
                        string.Format(
                            "UrlRewrite - Regex - Rule '{0}' failed.  Site '{1}' does not equal rules site condition '{2}'",
                            inboundRule.Name, currentSiteName, inboundRule.SiteNameRestriction), this);
                }
                else
                {
                    Log.Debug(
                        string.Format(
                            "UrlRewrite - Regex - Rule '{0}' matched site name restriction.  Site '{1}' equal rules site condition '{2}'",
                            inboundRule.Name, currentSiteName, inboundRule.SiteNameRestriction), this);
                }
            }
            else
            {
                Log.Warn(
                    string.Format(
                        "UrlRewrite - Regex - Rule '{0}' matching based on site name will not occur because site name is null.",
                        inboundRule.Name), this);
            }

            return isInboundRuleMatch;
        }

        private void ProcessActionProcessing(RuleResult ruleResult)
        {
            ruleResult.StopProcessing = true;
        }

        private void ProcessRedirectAction(InboundRule inboundRule, Uri uri, Match inboundRuleMatch, Match lastConditionMatch, RuleResult ruleResult)
        {
            var redirectAction = inboundRule.Action as RedirectAction;

            var rewriteUrl = redirectAction.RewriteUrl;
            var rewriteItemId = redirectAction.RewriteItemId;
            var rewriteItemAnchor = redirectAction.RewriteItemAnchor;

            if (string.IsNullOrEmpty(rewriteUrl) && rewriteItemId == null)
            {
                ruleResult.RuleMatched = false;
                return;
            }

            if (rewriteItemId.HasValue)
            {
                rewriteUrl = GetRewriteUrlFromItemId(rewriteItemId.Value, rewriteItemAnchor);
            }

            // process token replacements
            rewriteUrl = ReplaceTokens(uri, rewriteUrl);

            if (redirectAction.AppendQueryString)
            {
                rewriteUrl += uri.Query;
            }

            rewriteUrl = ReplaceRuleBackReferences(inboundRuleMatch, rewriteUrl);
            rewriteUrl = ReplaceConditionBackReferences(lastConditionMatch, rewriteUrl);

            ruleResult.RewrittenUri = new Uri(rewriteUrl);
            ruleResult.StopProcessing = redirectAction.StopProcessingOfSubsequentRules;
        }

        private void ProcessItemQueryRedirectAction(InboundRule inboundRule, Uri uri, Match inboundRuleMatch, Match lastConditionMatch, RuleResult ruleResult)
        {
            var redirectAction = inboundRule.Action as ItemQueryRedirectAction;

            var itemQuery = redirectAction.ItemQuery;

            if (string.IsNullOrEmpty(itemQuery))
            {
                ruleResult.RuleMatched = false;
                return;
            }

            // process token replacements in the item query
            itemQuery = ReplaceRuleBackReferences(inboundRuleMatch, itemQuery);
            itemQuery = ReplaceConditionBackReferences(lastConditionMatch, itemQuery);

            var rewriteItemId = ProcessItemQuery(itemQuery);

            if (!rewriteItemId.HasValue)
            {
                ruleResult.RuleMatched = false;
                return;
            }

            string rewriteUrl = GetRewriteUrlFromItemId(rewriteItemId.Value, null);

            // process token replacements
            rewriteUrl = ReplaceTokens(uri, rewriteUrl);
            rewriteUrl = ReplaceRuleBackReferences(inboundRuleMatch, rewriteUrl);
            rewriteUrl = ReplaceConditionBackReferences(lastConditionMatch, rewriteUrl);

            ruleResult.RewrittenUri = new Uri(rewriteUrl);
            ruleResult.StopProcessing = redirectAction.StopProcessingOfSubsequentRules;
        }

        private Guid? ProcessItemQuery(string itemQuery)
        {
            var db = Sitecore.Context.Database;
            var items = db.SelectItems(itemQuery);
            if (items != null && items.Any())
            {
                return items.First().ID.Guid;
            }

            return null;
        }

        private static string ReplaceRuleBackReferences(Match inboundRuleMatch, string input)
        {
            return ReplaceBackReferences(inboundRuleMatch, input, "R");
        }

        private static string ReplaceConditionBackReferences(Match conditionMatch, string input)
        {
            return ReplaceBackReferences(conditionMatch, input, "C");
        }

        private static string ReplaceBackReferences(Match match, string input, string backReferenceVariable)
        {
            var groupRegex = new Regex(@"({" + backReferenceVariable + @":(\d+)})", RegexOptions.None);

            foreach (Match groupMatch in groupRegex.Matches(input))
            {
                var num = groupMatch.Groups[2];
                var groupIndex = Convert.ToInt32(num.Value);
                var group = match.Groups[groupIndex];
                var matchText = groupMatch.ToString();
                var groupValue = group.Value;

                input = input.Replace(matchText, groupValue);
            }
            return input;
        }

        private string ReplaceTokens(Uri uri, string input)
        {

            // host replacement
            input = input.Replace(Tokens.HTTP_HOST.Formatted(), uri.Host);

            // querystring replacement
            var query = uri.Query;
            if (query.Length > 0)
            {
                query = query.Substring(1);
            }
            input = input.Replace(Tokens.QUERY_STRING.Formatted(), HttpUtility.UrlDecode(query));

            // scheme replacement
            var https = uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.InvariantCultureIgnoreCase) ? "on" : "off";
            input = input.Replace(Tokens.HTTPS.Formatted(), https);

            return input;
        }

        private string ReplaceTokenInput(string input, Tokens token)
        {
            input = input.Replace(token.Formatted(), ServerVariables[token.ToString()]);

            return input;
        }

        private string GetRewriteUrlFromItemId(Guid rewriteItemId, string rewriteItemAnchor)
        {
            string rewriteUrl = null;

            var db = Sitecore.Context.Database;
            if (db != null)
            {
                var rewriteItem = db.GetItem(new ID(rewriteItemId));

                if (rewriteItem != null)
                {
                    if (rewriteItem.Paths.IsMediaItem)
                    {
                        var mediaUrlOptions = new MediaUrlOptions
                        {
                            AlwaysIncludeServerUrl = true
                        };

                        rewriteUrl = MediaManager.GetMediaUrl(rewriteItem, mediaUrlOptions);
                    }
                    else
                    {
                        var urlOptions = LinkManager.GetDefaultUrlOptions();
                        urlOptions.AlwaysIncludeServerUrl = true;
                        urlOptions.SiteResolving = true;

                        rewriteUrl = LinkManager.GetItemUrl(rewriteItem, urlOptions);
                    }

                    if (!string.IsNullOrEmpty(rewriteItemAnchor))
                    {
                        rewriteUrl += string.Format("#{0}", rewriteItemAnchor);
                    }
                }
            }

            return rewriteUrl;
        }

        private Tuple<Match, string> ConditionMatch(Uri uri, Condition condition, Match previousConditionMatch = null)
        {
            var conditionRegex = new Regex(condition.Pattern, condition.IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);

            var conditionInput = ReplaceTokens(uri, condition.InputString);
            if (previousConditionMatch != null)
            {
                conditionInput = ReplaceConditionBackReferences(previousConditionMatch, conditionInput);
            }

            var returnMatch = conditionRegex.Match(conditionInput);

            return new Tuple<Match, string>(returnMatch, conditionInput);
        }

    }
}
