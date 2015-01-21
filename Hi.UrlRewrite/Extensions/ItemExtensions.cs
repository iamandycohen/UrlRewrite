using Hi.UrlRewrite.Entities.Actions;
using Hi.UrlRewrite.Entities.Actions.Base;
using Hi.UrlRewrite.Entities.Conditions;
using Hi.UrlRewrite.Entities.Match;
using Hi.UrlRewrite.Entities.Rules;
using Hi.UrlRewrite.Entities.ServerVariables;
using Hi.UrlRewrite.Processing;
using Hi.UrlRewrite.Templates;
using Hi.UrlRewrite.Templates.Action;
using Hi.UrlRewrite.Templates.Action.Base;
using Hi.UrlRewrite.Templates.Conditions;
using Hi.UrlRewrite.Templates.Folders;
using Hi.UrlRewrite.Templates.Inbound;
using Hi.UrlRewrite.Templates.Match;
using Hi.UrlRewrite.Templates.Outbound;
using Hi.UrlRewrite.Templates.ServerVariables;
using Sitecore.Data;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hi.UrlRewrite
{
    public static class ItemExtensions
    {

        #region Conversions

        #region Rules

        public static OutboundRule ToOutboundRule(this OutboundRuleItem outboundRuleItem)
        {
            if (outboundRuleItem == null) return null;

            var conditionItems = GetBaseConditionItems(outboundRuleItem);

            var outboundRule = new OutboundRule
            {
                ItemId = outboundRuleItem.ID.Guid,
                Name = outboundRuleItem.Name
            };

            SetBaseRule(outboundRuleItem.BaseRuleItem, outboundRule);

            SetOutboundMatch(outboundRuleItem.OutboundMatchItem, outboundRule);

            GetPrecondition(outboundRuleItem.OutboundPreconditionItem, outboundRule);

            if (string.IsNullOrEmpty(outboundRuleItem.BaseRuleItem.BaseMatchItem.MatchPatternItem.Pattern.Value))
            {
                Log.Warn(logObject, outboundRuleItem.Database, "No pattern set on rule with ItemID: {0}", outboundRuleItem.ID);

                return null;
            }

            if (outboundRuleItem.Action == null)
            {
                Log.Warn(logObject, outboundRuleItem.Database, "No action set on rule with ItemID: {0}", outboundRuleItem.ID);

                return null;
            }

            var baseActionItem = outboundRuleItem.Action.TargetItem;
            IBaseAction baseAction = null;

            if (baseActionItem != null)
            {
                var baseActionItemTemplateId = baseActionItem.TemplateID.ToString();

                if (baseActionItemTemplateId.Equals(OutboundRewriteItem.TemplateId, StringComparison.InvariantCultureIgnoreCase))
                {
                    baseAction = new OutboundRewriteItem(baseActionItem).ToOutboundRewriteAction();
                }
            }
            outboundRule.Action = baseAction;

            if (conditionItems != null)
            {
                SetConditions(conditionItems, outboundRule);
            }

            return outboundRule;
        }

        private static void GetPrecondition(OutboundPreconditionItem outboundPreconditionItem, OutboundRule outboundRule)
        {
            if (outboundPreconditionItem == null || outboundPreconditionItem.Precondition == null ||
                outboundPreconditionItem.Precondition.TargetItem == null) return;

            var preconditionTargetItem = outboundPreconditionItem.Precondition.TargetItem;
            var preconditionItem = new PreconditionItem(preconditionTargetItem);

            var precondition = new Precondition
            {
                Name = preconditionItem.Name,
            };

            var conditionItems = GetBaseConditionItems(preconditionItem);
            if (conditionItems != null)
            {
                SetConditions(conditionItems, precondition);
            }

            var usingItem = preconditionItem.PreconditionUsingItem.Using.TargetItem;
            Using? usingType = null;
            if (usingItem != null)
            {
                var usingItemId = usingItem.ID.ToString();
                switch (usingItemId)
                {
                    case Constants.UsingType_RegularExpressions_ItemId:
                        usingType = Using.RegularExpressions;
                        break;
                    case Constants.UsingType_Wildcards_ItemId:
                        usingType = Using.Wildcards;
                        break;
                    case Constants.UsingType_ExactMatch_ItemId:
                        usingType = Using.ExactMatch;
                        break;
                    default:
                        break;
                }
            }
            precondition.Using = usingType;

            SetConditionLogicalGrouping(preconditionItem.ConditionLogicalGroupingItem, precondition);

            outboundRule.Precondition = precondition;
        }

        public static InboundRule ToInboundRule(this InboundRuleItem inboundRuleItem, string siteNameRestriction)
        {

            if (inboundRuleItem == null) return null;

            var conditionItems = GetBaseConditionItems(inboundRuleItem);
            //var serverVariableItems = GetServerVariableItems(inboundRuleItem);
            //var requestHeaderItems = GetRequestHeaderItems(inboundRuleItem);
            var responseHeaderItems = GetResponseHeaderItems(inboundRuleItem);

            var inboundRule = new InboundRule
            {
                ItemId = inboundRuleItem.ID.Guid,
                Name = inboundRuleItem.Name
            };

            SetBaseRule(inboundRuleItem.BaseRuleItem, inboundRule);

            if (string.IsNullOrEmpty(inboundRuleItem.BaseRuleItem.BaseMatchItem.MatchPatternItem.Pattern.Value))
            {
                Log.Warn(logObject, inboundRuleItem.Database, "No pattern set on rule with ItemID: {0}", inboundRuleItem.ID);

                return null;
            }

            if (inboundRuleItem.Action == null)
            {
                Log.Warn(logObject, inboundRuleItem.Database, "No action set on rule with ItemID: {0}", inboundRuleItem.ID);

                return null;
            }

            var baseActionItem = inboundRuleItem.Action.TargetItem;
            IBaseAction baseAction = null;
            if (baseActionItem != null)
            {
                var baseActionItemTemplateId = baseActionItem.TemplateID.ToString();

                if (baseActionItemTemplateId.Equals(RedirectItem.TemplateId, StringComparison.InvariantCultureIgnoreCase))
                {
                    baseAction = new RedirectItem(baseActionItem).ToRedirectAction();
                }
                else if (baseActionItemTemplateId.Equals(RewriteItem.TemplateId,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    baseAction = new RewriteItem(baseActionItem).ToRewriteAction();
                }
                else if (baseActionItemTemplateId.Equals(AbortRequestItem.TemplateId,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    baseAction = new AbortRequestItem(baseActionItem).ToAbortRequestAction();
                }
                else if (baseActionItemTemplateId.Equals(CustomResponseItem.TemplateId,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    baseAction = new CustomResponseItem(baseActionItem).ToCustomResponseAction();
                }
                else if (baseActionItemTemplateId.Equals(ItemQueryRedirectItem.TemplateId))
                {
                    baseAction = new ItemQueryRedirectItem(baseActionItem).ToItemQueryRedirectAction();
                }
            }
            inboundRule.Action = baseAction;

            if (conditionItems != null)
            {
                SetConditions(conditionItems, inboundRule);
            }

            //if (serverVariableItems != null)
            //{
            //    SetServerVariables(serverVariableItems, inboundRule);
            //}

            //if (requestHeaderItems != null)
            //{
            //    SetRequestHeaders(requestHeaderItems, inboundRule);
            //}

            if (responseHeaderItems != null)
            {
                SetResponseHeaders(responseHeaderItems, inboundRule);
            }

            inboundRule.SiteNameRestriction = siteNameRestriction;

            return inboundRule;
        }

        public static IEnumerable<BaseConditionItem> GetBaseConditionItems(Item item)
        {
            IEnumerable<BaseConditionItem> conditionItems = null;

            var conditions =
                item.Axes.SelectItems(string.Format(Constants.TwoTemplateQuery,
                    ConditionItem.TemplateId, ConditionAdvancedItem.TemplateId));

            if (conditions != null)
            {
                conditionItems = conditions.Select(e => new BaseConditionItem(e));
            }

            return conditionItems;
        }

        public static IEnumerable<ServerVariableItem> GetServerVariableItems(Item item)
        {
            IEnumerable<ServerVariableItem> serverVariableItems = null;

            var serverVariables =
                item.Axes.SelectItems(string.Format(Constants.SingleTemplateQuery, ServerVariableItem.TemplateId));

            if (serverVariables != null)
            {
                serverVariableItems = serverVariables.Select(e => new ServerVariableItem(e));
            }

            return serverVariableItems;
        }

        public static IEnumerable<RequestHeaderItem> GetRequestHeaderItems(Item item)
        {
            IEnumerable<RequestHeaderItem> requestHeaderItems = null;

            var requestHeaders =
                item.Axes.SelectItems(string.Format(Constants.SingleTemplateQuery, RequestHeaderItem.TemplateId));

            if (requestHeaders != null)
            {
                requestHeaderItems = requestHeaders.Select(e => new RequestHeaderItem(e));
            }

            return requestHeaderItems;
        }

        public static IEnumerable<ResponseHeaderItem> GetResponseHeaderItems(Item item)
        {
            IEnumerable<ResponseHeaderItem> responeHeaderItems = null;

            var responseHeaders =
                item.Axes.SelectItems(string.Format(Constants.SingleTemplateQuery, ResponseHeaderItem.TemplateId));

            if (responseHeaders != null)
            {
                responeHeaderItems = responseHeaders.Select(e => new ResponseHeaderItem(e));
            }

            return responeHeaderItems;
        }

        private static void SetServerVariables(IEnumerable<ServerVariableItem> serverVariableItems, IServerVariableList serverVariableList)
        {
            var serverVariables = serverVariableItems
                .Select(e => e.ToServerVariable())
                .Where(e => e != null)
                .ToList();

            serverVariableList.ServerVariables = serverVariables;
        }

        private static void SetResponseHeaders(IEnumerable<ResponseHeaderItem> responseHeaderItems, IResponseHeaderList responseHeaderList)
        {
            var responseHeaders = responseHeaderItems
                .Select(e => e.ToResponseHeader())
                .Where(e => e != null)
                .ToList();

            responseHeaderList.ResponseHeaders = responseHeaders;
        }

        private static void SetRequestHeaders(IEnumerable<RequestHeaderItem> requestHeaderItems, IRequestHeaderList requestHeaderList)
        {
            var requestHeaders = requestHeaderItems
                .Select(e => e.ToRequestHeader())
                .Where(e => e != null)
                .ToList();

            requestHeaderList.RequestHeaders = requestHeaders;
        }

        private static void SetConditions(IEnumerable<BaseConditionItem> conditionItems, IConditionList conditionList)
        {
            conditionList.Conditions = conditionItems
                .Select(e => e.ToCondition())
                .Where(e => e != null)
                .ToList();
        }

        private static void SetBaseRule(BaseRuleItem baseRuleItem, IBaseRule baseRule)
        {
            SetBaseEnabled(baseRuleItem.InnerItem, baseRule);

            SetBaseMatch(baseRuleItem.BaseMatchItem, baseRule);

            SetConditionLogicalGrouping(baseRuleItem.ConditionLogicalGroupingItem, baseRule);
        }

        private static void SetBaseEnabled(BaseEnabledItem baseEnabledItem, IBaseEnabled baseEnabled)
        {
            baseEnabled.Enabled = baseEnabledItem.Enabled.Checked;
        }

        private static void SetConditionLogicalGrouping(ConditionLogicalGroupingItem conditionLogicalGroupingItem, IConditionLogicalGrouping conditionLogicalGrouping)
        {
            var logicalGroupingItem = conditionLogicalGroupingItem.LogicalGrouping.TargetItem;
            LogicalGrouping? logicalGroupingType = null;
            if (logicalGroupingItem != null)
            {
                var logicalGroupingItemId = logicalGroupingItem.ID.ToString();
                switch (logicalGroupingItemId)
                {
                    case Constants.LogicalGroupingType_MatchAll_ItemId:
                        logicalGroupingType = LogicalGrouping.MatchAll;
                        break;
                    case Constants.LogicalGroupingType_MatchAny_ItemId:
                        logicalGroupingType = LogicalGrouping.MatchAny;
                        break;
                    default:
                        break;
                }
            }
            conditionLogicalGrouping.ConditionLogicalGrouping = logicalGroupingType;
        }

        private static void SetBaseMatch(BaseMatchItem baseMatchItem, IBaseMatch baseMatch)
        {
            baseMatch.IgnoreCase = baseMatchItem.MatchIgnoreCaseItem.IgnoreCase.Checked;
            baseMatch.Pattern = baseMatchItem.MatchPatternItem.Pattern.Value;

            var matchTypeItem = baseMatchItem.MatchMatchTypeItem.MatchType.TargetItem;
            MatchType? matchType = null;
            if (matchTypeItem != null)
            {
                var requestUrlItemId = matchTypeItem.ID.ToString();
                switch (requestUrlItemId)
                {
                    case Constants.MatchType_MatchesThePattern_ItemId:
                        matchType = MatchType.MatchesThePattern;
                        break;
                    case Constants.MatchType_DoesNotMatchThePattern_ItemId:
                        matchType = MatchType.DoesNotMatchThePattern;
                        break;
                    default:
                        break;
                }
            }
            baseMatch.MatchType = matchType;

            var usingItem = baseMatchItem.MatchUsingItem.Using.TargetItem;
            Using? usingType = null;
            if (usingItem != null)
            {
                var usingItemId = usingItem.ID.ToString();
                switch (usingItemId)
                {
                    case Constants.UsingType_RegularExpressions_ItemId:
                        usingType = Using.RegularExpressions;
                        break;
                    case Constants.UsingType_Wildcards_ItemId:
                        usingType = Using.Wildcards;
                        break;
                    case Constants.UsingType_ExactMatch_ItemId:
                        usingType = Using.ExactMatch;
                        break;
                    default:
                        break;
                }
            }
            baseMatch.Using = usingType;
        }

        private static void SetOutboundMatch(OutboundMatchItem outboundMatchItem, IOutboundMatch outboundMatch)
        {
            SetBaseMatch(outboundMatchItem.BaseMatchItem, outboundMatch);

            var scopeTypeItem = outboundMatchItem.MatchScopeItem.MatchScopeType.TargetItem;
            if (scopeTypeItem != null)
            {
                var scopeTypeItemId = scopeTypeItem.ID;
                if (scopeTypeItemId.Equals(new ID(Constants.MatchScope_Response_ItemId)))
                {
                    outboundMatch.MatchingScopeType = ScopeType.Response;
                } 
                else if (scopeTypeItemId.Equals(new ID(Constants.MatchScope_ServerVariables_ItemId))) 
                {
                    outboundMatch.MatchingScopeType = ScopeType.ServerVariables;
                }

            }

            var outboundMatchScopeItem = outboundMatchItem.OutboundMatchScopeItem.MatchScope.TargetItem;
            if (outboundMatchScopeItem != null)
            {
                if (outboundMatchScopeItem.TemplateID.Equals(new ID(MatchResponseTagsItem.TemplateId)))
                {

                    var matchTags = new MatchResponseTagsItem(outboundMatchScopeItem).MatchTheContentWithin.TargetIDs
                        .Select(i => outboundMatchItem.InnerItem.Database.GetItem(i))
                        .Select(i => new MatchTagItem(i))
                        .Where(i => i != null)
                        .Select(i => new MatchTag { Tag = i.Tag.Value, Attribute = i.Attribute.Value})
                        .ToList();

                    outboundMatch.OutboundMatchScope = new MatchResponseTags()
                    {
                        MatchTheContentWithin = matchTags
                    };
                }
                else if (outboundMatchScopeItem.TemplateID.Equals(new ID(MatchServerVariableItem.TemplateId)))
                {
                    var serverVariableItem = new MatchServerVariableItem(outboundMatchScopeItem);
                    outboundMatch.OutboundMatchScope = new MatchServerVariable
                    {
                        ServerVariableName = serverVariableItem.ServerVariableName.Value
                    };
                }
            }

        }

        #endregion

        #region Conditions

        public static Condition ToCondition(this BaseConditionItem baseConditionItem)
        {
            var condition = GetBaseConditionInfo(baseConditionItem);

            if (condition == null) return null;

            var baseConditionItemTemplateId = baseConditionItem.InnerItem.TemplateID;

            if (baseConditionItemTemplateId.Equals(new ID(ConditionItem.TemplateId)))
            {
                var conditionInputItem = new ConditionItem(baseConditionItem).ConditionInputType.TargetItem;
                Tokens? conditionInputType = null;

                if (conditionInputItem != null)
                {
                    switch (conditionInputItem.ID.ToString())
                    {
                        case Constants.ConditionInputType_QueryString_ItemId:
                            conditionInputType = Tokens.QUERY_STRING;
                            break;
                        case Constants.ConditionInputType_HttpHost_ItemId:
                            conditionInputType = Tokens.HTTP_HOST;
                            break;
                        case Constants.ConditionInputType_Https_ItemId:
                            conditionInputType = Tokens.HTTPS;
                            break;
                        default:
                            break;
                    }
                }

                condition.InputString = string.Format("{{{0}}}", conditionInputType);
            }
            else if (baseConditionItemTemplateId.Equals(new ID(ConditionAdvancedItem.TemplateId)))
            {
                condition.InputString = new ConditionAdvancedItem(baseConditionItem).ConditionInputString.Value;
            }

            return condition;
        }

        private static Condition GetBaseConditionInfo(BaseConditionItem conditionItem)
        {
            if (conditionItem == null)
            {
                return null;
            }

            var condition = new Condition
            {
                Name = conditionItem.Name,
                Pattern = conditionItem.Pattern.Value,
                IgnoreCase = conditionItem.IgnoreCase.Checked
            };

            var checkIfInputStringItem = conditionItem.CheckIfInputString.TargetItem;
            CheckIfInputString? checkIfInputStringType = null;

            if (checkIfInputStringItem != null)
            {
                switch (checkIfInputStringItem.ID.ToString())
                {
                    case Constants.CheckIfInputStringType_IsAFile_ItemId:
                        checkIfInputStringType = CheckIfInputString.IsAFile;
                        break;
                    case Constants.CheckIfInputStringType_IsNotAFile_ItemId:
                        checkIfInputStringType = CheckIfInputString.IsNotAFile;
                        break;
                    case Constants.CheckIfInputStringType_IsADirectory_ItemId:
                        checkIfInputStringType = CheckIfInputString.IsADirectory;
                        break;
                    case Constants.CheckIfInputStringType_IsNotADirectory_ItemId:
                        checkIfInputStringType = CheckIfInputString.IsNotADirctory;
                        break;
                    case Constants.CheckIfInputStringType_MatchesThePattern_ItemId:
                        checkIfInputStringType = CheckIfInputString.MatchesThePattern;
                        break;
                    case Constants.CheckIfInputStringType_DoesNotMatchThePattern_ItemId:
                        checkIfInputStringType = CheckIfInputString.DoesNotMatchThePattern;
                        break;
                    default:
                        break;
                }
            }
            condition.CheckIfInputString = checkIfInputStringType;

            return condition;
        }

        #endregion

        #region Server Variables

        public static ServerVariable ToServerVariable(this ServerVariableItem serverVariableItem)
        {
            var serverVariable = GetBaseServerVariable(serverVariableItem);

            return serverVariable as ServerVariable;
        }

        public static RequestHeader ToRequestHeader(this RequestHeaderItem requestHeaderItem)
        {
            var requestHeader = GetBaseServerVariable(requestHeaderItem);

            return requestHeader as RequestHeader;
        }

        public static ResponseHeader ToResponseHeader(this ResponseHeaderItem responseHeaderItem)
        {
            var responseHeader = GetBaseServerVariable(responseHeaderItem);

            return responseHeader as ResponseHeader;
        }

        private static IBaseServerVariable GetBaseServerVariable(Item variableItem)
        {
            if (variableItem == null)
            {
                return null;
            }

            var templateId = variableItem.TemplateID;
            var baseServerVariableItem = new BaseServerVariableItem(variableItem);
            var variableName = baseServerVariableItem.VariableName.Value;
            var name = baseServerVariableItem.Name;
            var value = baseServerVariableItem.Value.Value;
            var replaceExistingValue = baseServerVariableItem.ReplaceExistingValue.Checked;

            if (templateId.Equals(new ID(ServerVariableItem.TemplateId)))
            {
                return new ServerVariable
                {
                    Name = name,
                    VariableName = variableName,
                    Value = value,
                    ReplaceExistingValue = replaceExistingValue
                };
            }

            if (templateId.Equals(new ID(RequestHeaderItem.TemplateId)))
            {
                return new RequestHeader
                {
                    Name = name,
                    VariableName = variableName,
                    Value = value,
                    ReplaceExistingValue = replaceExistingValue
                };
            }

            if (templateId.Equals(new ID(ResponseHeaderItem.TemplateId)))
            {
                return new ResponseHeader
                {
                    Name = name,
                    VariableName = variableName,
                    Value = value,
                    ReplaceExistingValue = replaceExistingValue
                };
            }

            return null;
        }

        #endregion

        #region Actions

        public static Redirect ToRedirectAction(this RedirectItem redirectItem)
        {

            if (redirectItem == null)
            {
                return null;
            }

            var redirectAction = new Redirect
            {
                Name = redirectItem.Name
            };

            GetBaseRewriteUrlItem(redirectItem.BaseRedirectItem.BaseRewriteUrlItem, redirectAction);

            var baseAppendQueryString = redirectItem.BaseRedirectItem.BaseAppendQuerystringItem;
            GetBaseAppendQueryStringItem(baseAppendQueryString, redirectAction);

            var stopProcessingItem = redirectItem.BaseRedirectItem.BaseStopProcessingItem;
            GetStopProcessing(stopProcessingItem, redirectAction);

            var redirectTypeItem = redirectItem.BaseRedirectItem.BaseRedirectTypeItem;
            GetStatusCode(redirectTypeItem, redirectAction);

            var httpCacheabilityTypeItem = redirectItem.BaseRedirectItem.BaseCacheItem;
            GetCacheability(httpCacheabilityTypeItem, redirectAction);

            return redirectAction;
        }

        public static Rewrite ToRewriteAction(this RewriteItem rewriteItem)
        {
            if (rewriteItem == null)
            {
                return null;
            }

            var rewriteAction = new Rewrite
            {
                Name = rewriteItem.Name
            };

            GetBaseRewriteUrlItem(rewriteItem.BaseRewriteItem.BaseRewriteUrlItem, rewriteAction);

            var baseAppendQueryString = rewriteItem.BaseRewriteItem.BaseAppendQuerystringItem;
            GetBaseAppendQueryStringItem(baseAppendQueryString, rewriteAction);

            var stopProcessingItem = rewriteItem.BaseRewriteItem.BaseStopProcessingItem;
            GetStopProcessing(stopProcessingItem, rewriteAction);

            return rewriteAction;
        }

        public static AbortRequest ToAbortRequestAction(this AbortRequestItem abortRequestItem)
        {
            if (abortRequestItem == null)
            {
                return null;
            }

            var abortRequestAction = new AbortRequest()
            {
                Name = abortRequestItem.Name
            };

            return abortRequestAction;
        }

        public static OutboundRewrite ToOutboundRewriteAction(this OutboundRewriteItem outboundRewriteItem)
        {
            if (outboundRewriteItem == null)
            {
                return null;
            }

            var outboundRewriteAction = new OutboundRewrite()
            {
                Name = outboundRewriteItem.Name,
                Value = outboundRewriteItem.Value.Value
            };

            var stopProcessingItem = outboundRewriteItem.BaseStopProcessingItem;
            GetStopProcessing(stopProcessingItem, outboundRewriteAction);

            return outboundRewriteAction;
        }

        public static CustomResponse ToCustomResponseAction(this CustomResponseItem customResponseItem)
        {
            if (customResponseItem == null)
            {
                return null;
            }

            var customResponseAction = new CustomResponse()
            {
                Name = customResponseItem.Name,
            };

            var statusCode = 0;

            if (!int.TryParse(customResponseItem.StatusCode.Value, out statusCode))
            {
                return null;
            }

            customResponseAction.StatusCode = statusCode;

            if (customResponseItem.SubstatusCode.Value != null)
            {
                int outSubStatusCode = 0;
                if (int.TryParse(customResponseItem.SubstatusCode.Value, out outSubStatusCode))
                {
                    customResponseAction.SubStatusCode = outSubStatusCode;
                }
            }

            customResponseAction.ErrorDescription = customResponseItem.ErrorDescription.Value;
            customResponseAction.Reason = customResponseItem.Reason.Value;

            return customResponseAction;
        }

        public static ItemQueryRedirect ToItemQueryRedirectAction(this ItemQueryRedirectItem itemQueryRedirectItem)
        {
            if (itemQueryRedirectItem == null)
            {
                return null;
            }

            var itemQueryRedirectAction = new ItemQueryRedirect()
            {
                Name = itemQueryRedirectItem.Name,
                ItemQuery = itemQueryRedirectItem.ItemQuery.Value
            };

            var baseAppendQueryString = itemQueryRedirectItem.BaseAppendQuerystringItem;
            GetBaseAppendQueryStringItem(baseAppendQueryString, itemQueryRedirectAction);

            var stopProcessingItem = itemQueryRedirectItem.BaseStopProcessingItem;
            GetStopProcessing(stopProcessingItem, itemQueryRedirectAction);

            var redirectTypeItem = itemQueryRedirectItem.BaseRedirectTypeItem;
            GetStatusCode(redirectTypeItem, itemQueryRedirectAction);

            var httpCacheabilityTypeItem = itemQueryRedirectItem.BaseCacheItem;
            GetCacheability(httpCacheabilityTypeItem, itemQueryRedirectAction);

            return itemQueryRedirectAction;
        }

        private static void GetStopProcessing(BaseStopProcessingItem redirectItem, IBaseStopProcessing redirectAction)
        {
            redirectAction.StopProcessingOfSubsequentRules =
                redirectItem.StopProcessingOfSubsequentRules.Checked;
        }

        private static void GetBaseRewriteUrlItem(BaseRewriteUrlItem baseRewriteUrlItem, IBaseRewriteUrl redirectAction)
        {
            var redirectTo = baseRewriteUrlItem.RewriteUrl;
            string actionRewriteUrl;
            Guid? redirectItemId;
            string redirectItemAnchor;

            RulesEngine.GetRedirectUrlOrItemId(redirectTo, out actionRewriteUrl, out redirectItemId, out redirectItemAnchor);
            redirectAction.RewriteItemId = redirectItemId;
            redirectAction.RewriteItemAnchor = redirectItemAnchor;
            redirectAction.RewriteUrl = actionRewriteUrl;
        }

        private static void GetBaseAppendQueryStringItem(BaseAppendQuerystringItem baseAppendQueryString, IBaseAppendQueryString redirectAction)
        {
            redirectAction.AppendQueryString = baseAppendQueryString.AppendQueryString.Checked;
        }

        private static void GetCacheability(BaseCacheItem httpCacheabilityTypeItem, IBaseCache redirectAction)
        {
            var httpCacheabilityTypeTargetItem = httpCacheabilityTypeItem.HttpCacheability.TargetItem;
            HttpCacheability? httpCacheability = null;
            if (httpCacheabilityTypeTargetItem != null)
            {
                switch (httpCacheabilityTypeTargetItem.ID.ToString())
                {
                    case Constants.HttpCacheabilityType_NoCache_ItemId:
                        httpCacheability = HttpCacheability.NoCache;
                        break;
                    case Constants.HttpCacheabilityType_Private_ItemId:
                        httpCacheability = HttpCacheability.Private;
                        break;
                    case Constants.HttpCacheabilityType_Server_ItemId:
                        httpCacheability = HttpCacheability.Server;
                        break;
                    case Constants.HttpCacheabilityType_ServerAndNoCache_ItemId:
                        httpCacheability = HttpCacheability.ServerAndNoCache;
                        break;
                    case Constants.HttpCacheabilityType_Public_ItemId:
                        httpCacheability = HttpCacheability.Public;
                        break;
                    case Constants.HttpCacheabilityType_ServerAndPrivate_ItemId:
                        httpCacheability = HttpCacheability.ServerAndPrivate;
                        break;
                    default:
                        break;
                }
            }
            redirectAction.HttpCacheability = httpCacheability;
        }

        private static void GetStatusCode(BaseRedirectTypeItem redirectTypeItem, IBaseStatusCode redirectAction)
        {
            var redirectTypeTargetItem = redirectTypeItem.RedirectType.TargetItem;
            RedirectStatusCode? redirectType = null;
            if (redirectTypeTargetItem != null)
            {
                switch (redirectTypeTargetItem.ID.ToString())
                {
                    case Constants.RedirectType_Permanent_ItemId:
                        redirectType = RedirectStatusCode.Permanent;
                        break;
                    case Constants.RedirectType_Found_ItemId:
                        redirectType = RedirectStatusCode.Found;
                        break;
                    case Constants.RedirectType_SeeOther_ItemId:
                        redirectType = RedirectStatusCode.SeeOther;
                        break;
                    case Constants.RedirectType_Temporary_ItemId:
                        redirectType = RedirectStatusCode.Temporary;
                        break;
                    default:
                        break;
                }
            }
            redirectAction.StatusCode = redirectType;
        }

        #endregion

        #endregion

        #region Helpers

        public static bool IsOutboundRuleItem(this Item item)
        {
            return !IsTemplate(item) && item.TemplateID.ToString().Equals(OutboundRuleItem.TemplateId, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsRedirectFolderItem(this Item item)
        {
            return !IsTemplate(item) && item.TemplateID.ToString().Equals(RedirectFolderItem.TemplateId, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsSimpleRedirectItem(this Item item)
        {
            return !IsTemplate(item) && item.TemplateID.ToString().Equals(SimpleRedirectItem.TemplateId, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsInboundRuleItem(this Item item)
        {
            return !IsTemplate(item) && item.TemplateID.ToString().Equals(InboundRuleItem.TemplateId, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsInboundRuleItemChild(this Item item)
        {
            if (item.Parent != null)
            {
                return !IsTemplate(item) && item.Parent.TemplateID.ToString().Equals(InboundRuleItem.TemplateId, StringComparison.InvariantCultureIgnoreCase);
            }
            return false;
        }

        public static bool IsOutboundRuleItemChild(this Item item)
        {
            if (item.Parent != null)
            {
                return !IsTemplate(item) && item.Parent.TemplateID.ToString().Equals(OutboundRuleItem.TemplateId, StringComparison.InvariantCultureIgnoreCase);
            }
            return false;
        }

        public static bool IsRedirectType(this Item item)
        {
            return !IsTemplate(item) && (new ID[]
                   {
                       new ID(RewriteItem.TemplateId),
                       new ID(RedirectItem.TemplateId),
                       new ID(ItemQueryRedirectItem.TemplateId),
                       new ID(CustomResponseItem.TemplateId),
                       new ID(AbortRequestItem.TemplateId),
                       new ID(NoneItem.TemplateId)
                   }).Any(e => e.Equals(item.TemplateID));
        }

        public static bool IsTemplate(this Item item)
        {
            return item.Paths.FullPath.StartsWith("/sitecore/templates", StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion

        #region LogObject

        private static LogObject logObject = new LogObject();

        private class LogObject
        {
        }

        #endregion

    }
}
