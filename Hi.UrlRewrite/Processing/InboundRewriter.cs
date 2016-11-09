using Hi.UrlRewrite.Analytics;
using Hi.UrlRewrite.Entities.Actions;
using Hi.UrlRewrite.Entities.Actions.Base;
using Hi.UrlRewrite.Entities.Match;
using Hi.UrlRewrite.Entities.Rules;
using Hi.UrlRewrite.Processing.Results;
using Sitecore.Data;
using Sitecore.Links;
using Sitecore.Resources.Media;
using Sitecore.Sites;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace Hi.UrlRewrite.Processing
{
    public class InboundRewriter
    {

        public NameValueCollection RequestServerVariables { get; set; }
        public NameValueCollection RequestHeaders { get; set; }

        public InboundRewriter()
        {
            RequestServerVariables = new NameValueCollection();
            RequestHeaders = new NameValueCollection();
        }

        public InboundRewriter(NameValueCollection requestServerVariables, NameValueCollection requestHeaders)
        {
            RequestServerVariables = requestServerVariables;
            RequestHeaders = requestHeaders;
        }

        public ProcessInboundRulesResult ProcessRequestUrl(Uri requestUri, List<InboundRule> inboundRules)
        {
            if (inboundRules == null)
            {
                throw new ArgumentNullException("inboundRules");
            }

            var originalUri = requestUri;

            Log.Debug(this, "Processing url: {0}", originalUri);

            var ruleResult = new InboundRuleResult
            {
                RewrittenUri = originalUri
            };

            var processedResults = new List<InboundRuleResult>();

            foreach (var inboundRule in inboundRules)
            {
                ruleResult = ProcessInboundRule(ruleResult.RewrittenUri, inboundRule);
                processedResults.Add(ruleResult);

                if (!ruleResult.RuleMatched)
                    continue;

                if (ruleResult.RuleMatched && ruleResult.StopProcessing)
                {
                    break;
                }
            }

            // TODO: log more information about what actually hapenned - this currently only reflects rewrites/redirects
            Log.Debug(this, "Processed originalUrl: {0} redirectedUrl: {1}", originalUri, ruleResult.RewrittenUri);

            var finalResult = new ProcessInboundRulesResult(originalUri, processedResults);

            return finalResult;
        }

        public ProcessInboundRulesResult ProcessRequestUrl(Uri requestUri, List<InboundRule> inboundRules, SiteContext siteContext)
        {
            using (new SiteContextSwitcher(siteContext))
            {
                return ProcessRequestUrl(requestUri, inboundRules);
            }
        }

        public ProcessInboundRulesResult ProcessRequestUrl(Uri requestUri, List<InboundRule> inboundRules, Database database)
        {
            using (new DatabaseSwitcher(database))
            {
                return ProcessRequestUrl(requestUri, inboundRules);
            }
        }

        public ProcessInboundRulesResult ProcessRequestUrl(Uri requestUri, List<InboundRule> inboundRules, SiteContext siteContext, Database database)
        {
            using (new SiteContextSwitcher(siteContext))
            using (new DatabaseSwitcher(database))
            {
                return ProcessRequestUrl(requestUri, inboundRules);
            }
        }

        public void ExecuteResult(HttpContextBase httpContext, ProcessInboundRulesResult ruleResult)
        {
            var httpRequest = httpContext.Request;
            var httpResponse = httpContext.Response;

            var responseHeaders = ruleResult.ProcessedResults.SelectMany(e => e.ResponseHeaders);
            var replacements = new RewriteHelper.Replacements
            {
                RequestHeaders = RequestHeaders,
                RequestServerVariables = RequestServerVariables
            };

            httpResponse.Clear();

            foreach (var responseHeader in responseHeaders)
            {
                var responseHeaderValue = RewriteHelper.ReplaceTokens(replacements, responseHeader.Value);
                httpResponse.Headers.Set(responseHeader.VariableName, responseHeaderValue);
            }

            if (ruleResult.FinalAction is IBaseRedirect)
            {
                if (Configuration.AnalyticsTrackingEnabled)
                {
                    Tracking.TrackRedirect(ruleResult);
                }

                var redirectAction = ruleResult.FinalAction as IBaseRedirect;
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
            else if (ruleResult.FinalAction is IBaseRewrite)
            {

                var rewrittenUrl = ruleResult.RewrittenUri;

                var isLocal = String.Equals(httpRequest.Url.Host, rewrittenUrl.Host,
                            StringComparison.OrdinalIgnoreCase);

                if (!isLocal)
                {
                    throw new ApplicationException("Rewrite Url must be a local URL");
                }

                httpContext.Server.TransferRequest(rewrittenUrl.PathAndQuery, true, httpRequest.HttpMethod, RequestHeaders, true);
            }
            else if (ruleResult.FinalAction is AbortRequest)
            {
                // do nothing
            }
            else if (ruleResult.FinalAction is CustomResponse)
            {
                var customResponse = ruleResult.FinalAction as CustomResponse;

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

            httpResponse.End();
        }

        private InboundRuleResult ProcessInboundRule(Uri originalUri, InboundRule inboundRule)
        {
            Log.Debug(this, "Processing inbound rule - requestUri: {0} inboundRule: {1}", originalUri, inboundRule.Name);

            var ruleResult = new InboundRuleResult
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

            Log.Debug(this, "Processing inbound rule - requestUri: {0} inboundRule: {1} rewrittenUrl: {2}", ruleResult.OriginalUri, inboundRule.Name, ruleResult.RewrittenUri);

            ruleResult.ItemId = inboundRule.ItemId;

            return ruleResult;
        }

        private InboundRuleResult ProcessRegularExpressionInboundRule(Uri originalUri, InboundRule inboundRule)
        {

            var ruleResult = new InboundRuleResult
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
            if (isInboundRuleMatch && inboundRule.Conditions != null && inboundRule.Conditions.Any())
            {
                var replacements = new RewriteHelper.Replacements
                {
                    RequestHeaders = RequestHeaders,
                    RequestServerVariables = RequestServerVariables
                };

                conditionMatchResult = RewriteHelper.TestConditionMatches(inboundRule, replacements, out lastConditionMatch);
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

                if (inboundRule.ResponseHeaders.Any())
                {
                    ruleResult.ResponseHeaders = inboundRule.ResponseHeaders;
                }

                Log.Debug(this, "INBOUND RULE MATCH - requestUri: {0} inboundRule: {1}", originalUri, inboundRule.Name);

                // TODO: Need to implement Rewrite, None

                if (inboundRule.Action is Redirect)
                {
                    ProcessRedirectAction(inboundRule, originalUri, inboundRuleMatch, lastConditionMatch, ruleResult);
                }
                else if (inboundRule.Action is Rewrite)
                {
                    ProcessRewriteAction(inboundRule, originalUri, inboundRuleMatch, lastConditionMatch, ruleResult);
                }
                else if (inboundRule.Action is ItemQueryRedirect)
                {
                    ProcessItemQueryRedirectAction(inboundRule, originalUri, inboundRuleMatch, lastConditionMatch, ruleResult);
                }
                else if (inboundRule.Action is AbortRequest || inboundRule.Action is CustomResponse)
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
                Log.Warn(this, "Inbound Rule has no Action set - inboundRule: {0} inboundRule ItemId: {1}", inboundRule.Name, inboundRule.ItemId);

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
            var matchesThePattern = inboundRule.MatchType.HasValue &&
                                    inboundRule.MatchType.Value == MatchType.MatchesThePattern;

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

            Log.Debug(this, "Regex - Pattern: '{0}' Input: '{1}' Success: {2}", pattern, uriPath,
                    isInboundRuleMatch);

            if (!isInboundRuleMatch && !uriPath.Equals(escapedUriPath, StringComparison.InvariantCultureIgnoreCase))
            {
                inboundRuleMatch = inboundRuleRegex.Match(escapedUriPath);
                isInboundRuleMatch = matchesThePattern ? inboundRuleMatch.Success : !inboundRuleMatch.Success;

                Log.Debug(this, "Regex - Pattern: '{0}' Input: '{1}' Success: {2}", pattern, escapedUriPath,
                        isInboundRuleMatch);
            }
            return isInboundRuleMatch;
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
                    Log.Debug(this, "Regex - Rule '{0}' failed.  Site '{1}' does not equal rules site condition '{2}'",
                            inboundRule.Name, currentSiteName, inboundRule.SiteNameRestriction);
                }
                else
                {
                    Log.Debug(this, "Regex - Rule '{0}' matched site name restriction.  Site '{1}' equal rules site condition '{2}'",
                            inboundRule.Name, currentSiteName, inboundRule.SiteNameRestriction);
                }
            }
            else
            {
                Log.Warn(this, "Regex - Rule '{0}' matching based on site name will not occur because site name is null.",
                        inboundRule.Name);
            }

            return isInboundRuleMatch;
        }

        private void ProcessActionProcessing(InboundRuleResult ruleResult)
        {
            ruleResult.StopProcessing = true;
        }

        private void ProcessRedirectAction(InboundRule inboundRule, Uri uri, Match inboundRuleMatch, Match lastConditionMatch, InboundRuleResult ruleResult)
        {
            var redirectAction = inboundRule.Action as Redirect;

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
            var replacements = new RewriteHelper.Replacements
            {
                RequestHeaders = RequestHeaders,
                RequestServerVariables = RequestServerVariables
            };

            rewriteUrl = RewriteHelper.ReplaceTokens(replacements, rewriteUrl);

            if (redirectAction.AppendQueryString)
            {
                rewriteUrl += uri.Query;
            }

            rewriteUrl = RewriteHelper.ReplaceRuleBackReferences(inboundRuleMatch, rewriteUrl);
            rewriteUrl = RewriteHelper.ReplaceConditionBackReferences(lastConditionMatch, rewriteUrl);

            ruleResult.RewrittenUri = new Uri(rewriteUrl);
            ruleResult.StopProcessing = redirectAction.StopProcessingOfSubsequentRules;
        }

        private void ProcessRewriteAction(InboundRule inboundRule, Uri uri, Match inboundRuleMatch, Match lastConditionMatch, InboundRuleResult ruleResult)
        {
            var redirectAction = inboundRule.Action as Rewrite;

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
            var replacements = new RewriteHelper.Replacements
            {
                RequestHeaders = RequestHeaders,
                RequestServerVariables = RequestServerVariables
            };

            rewriteUrl = RewriteHelper.ReplaceTokens(replacements, rewriteUrl);

            if (redirectAction.AppendQueryString)
            {
                rewriteUrl += uri.Query;
            }

            rewriteUrl = RewriteHelper.ReplaceRuleBackReferences(inboundRuleMatch, rewriteUrl);
            rewriteUrl = RewriteHelper.ReplaceConditionBackReferences(lastConditionMatch, rewriteUrl);

            ruleResult.RewrittenUri = new Uri(rewriteUrl);
            ruleResult.StopProcessing = redirectAction.StopProcessingOfSubsequentRules;
        }

        private void ProcessItemQueryRedirectAction(InboundRule inboundRule, Uri uri, Match inboundRuleMatch, Match lastConditionMatch, InboundRuleResult ruleResult)
        {
            var redirectAction = inboundRule.Action as ItemQueryRedirect;

            var itemQuery = redirectAction.ItemQuery;

            if (string.IsNullOrEmpty(itemQuery))
            {
                ruleResult.RuleMatched = false;
                return;
            }

            // process token replacements in the item query
            itemQuery = RewriteHelper.ReplaceRuleBackReferences(inboundRuleMatch, itemQuery);
            itemQuery = RewriteHelper.ReplaceConditionBackReferences(lastConditionMatch, itemQuery);

            var rewriteItemId = ExecuteItemQuery(itemQuery);

            if (!rewriteItemId.HasValue)
            {
                ruleResult.RuleMatched = false;
                return;
            }

            string rewriteUrl = GetRewriteUrlFromItemId(rewriteItemId.Value, null);


            // process token replacements
            var replacements = new RewriteHelper.Replacements
            {
                RequestHeaders = RequestHeaders,
                RequestServerVariables = RequestServerVariables
            };

            rewriteUrl = RewriteHelper.ReplaceTokens(replacements, rewriteUrl);
            rewriteUrl = RewriteHelper.ReplaceRuleBackReferences(inboundRuleMatch, rewriteUrl);
            rewriteUrl = RewriteHelper.ReplaceConditionBackReferences(lastConditionMatch, rewriteUrl);

            ruleResult.RewrittenUri = new Uri(rewriteUrl);
            ruleResult.StopProcessing = redirectAction.StopProcessingOfSubsequentRules;
        }

        private Guid? ExecuteItemQuery(string itemQuery)
        {
            var db = Sitecore.Context.Database;
            var items = db.SelectItems(itemQuery);
            if (items != null && items.Any())
            {
                return items.First().ID.Guid;
            }

            return null;
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



    }
}
