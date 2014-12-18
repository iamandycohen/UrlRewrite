using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Hi.UrlRewrite.Entities.Actions;
using Hi.UrlRewrite.Entities.Conditions;
using Hi.UrlRewrite.Entities.Rules;
using Hi.UrlRewrite.Processing;
using Hi.UrlRewrite.Processing.Results;
using Sitecore.Data;
using Sitecore.Data.Query;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Sites;

namespace Hi.UrlRewrite.sitecore_modules.Shell.UrlRewrite
{
    public partial class Default : System.Web.UI.Page
    {
        private Database _db;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
            else
            {
                Page.Validate();

                if (Page.IsValid)
                {
                    divFormGroup.Attributes["class"] = "form-group";
                    divTable.Visible = true;

                    var rewriter = new UrlRewriter();
                    _db = Sitecore.Context.ContentDatabase;

                    var rulesEngine = new RulesEngine();
                    var inboundRules = rulesEngine.GetInboundRules(_db);

                    ProcessRequestResult results;

                    var requestUri = new Uri(txtUrl.Text);
                    var siteContext = SiteContextFactory.GetSiteContext(requestUri.Host, requestUri.AbsolutePath,
                        requestUri.Port);

                    using (new SiteContextSwitcher(siteContext))
                    using (new DatabaseSwitcher(_db))
                    {
                        results = rewriter.ProcessRequestUrl(new Uri(txtUrl.Text), inboundRules);

                    }

                    if (results == null)
                    {
                        return;
                    }

                    resultsRepeater.DataSource = results.ProcessedResults;
                    resultsRepeater.DataBind();

                    var isAbort = results.FinalAction is AbortRequestAction;
                    var isCustomResponse = results.FinalAction is CustomResponseAction;

                    if (isAbort)
                    {
                        txtFinalUrl.InnerText = "Aborted";
                    }
                    else if (isCustomResponse)
                    {
                        var customResponse = results.FinalAction as CustomResponseAction;
                        const string resultFormat = "Custom Response: {0} {1} {2}";
                        txtFinalUrl.InnerText = string.Format(resultFormat, customResponse.StatusCode,
                            customResponse.SubStatusCode, customResponse.ErrorDescription);
                    }
                    else
                    {
                        if (!results.MatchedAtLeastOneRule)
                        {
                            txtFinalUrl.InnerText = "No matches.";
                        }
                        else
                        {
                            const string resultFormat = "Redirected to {0}.";
                            txtFinalUrl.InnerText = string.Format(resultFormat, results.RewrittenUri.ToString());
                        }
                    }
                }
                else
                {
                    if (!vldTxtUrl.IsValid)
                    {
                        divFormGroup.Attributes["class"] = "form-group has-error";
                    }
                }
            }
        }

        protected void resultsRepeater_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                if (e.Item.DataItem == null)
                    return;

                var result = e.Item.DataItem as RuleResult;
                if (result != null)
                {
                    var ruleMatched = result.RuleMatched;
                    var isAbort = ruleMatched && result.ResultAction is AbortRequestAction;
                    var isCustomResponse = ruleMatched && result.ResultAction is CustomResponseAction;
                    var conditionMatched = result.ConditionMatchResult != null && result.ConditionMatchResult.Matched;
                    var itemId = new ID(result.ItemId).ToShortID().ToString();

                    if (ruleMatched)
                    {
                        var tableRow = e.Item.FindControl("tableRow") as HtmlTableRow;
                        if (tableRow != null)
                        {
                            var tableRowClass = "success rule-row";
                            if (conditionMatched)
                            {
                                tableRowClass += " table-hover condition-hidden";
                            }

                            tableRow.Attributes["class"] = tableRowClass;
                            tableRow.Attributes["data-itemid"] = itemId;
                        }

                        var cellAction = e.Item.FindControl("cellAction") as HtmlTableCell;
                        if (cellAction != null)
                        {
                            if (isAbort)
                            {
                                cellAction.InnerText = "Abort";
                            }
                            else if (isCustomResponse)
                            {
                                cellAction.InnerText = "Custom Response";
                            }
                            else
                            {
                                cellAction.InnerText = "Redirect";
                            }
                        }
                    }

                    var item = _db.GetItem(new ID(result.ItemId));
                    if (item != null)
                    {
                        var cellName = e.Item.FindControl("cellName") as HtmlTableCell;
                        if (cellName != null) cellName.InnerText = item.Name;

                        var cellPath = e.Item.FindControl("cellPath") as HtmlTableCell;
                        if (cellPath != null) cellPath.InnerText = item.Paths.ContentPath;
                    }


                    var cellOriginalUrl = e.Item.FindControl("cellOriginalUrl") as HtmlTableCell;
                    if (cellOriginalUrl != null) cellOriginalUrl.InnerText = result.OriginalUri.ToString();

                    var cellRewrittenUrl = e.Item.FindControl("cellRewrittenUrl") as HtmlTableCell;
                    if (cellRewrittenUrl != null) cellRewrittenUrl.InnerText = result.RewrittenUri.ToString();

                    var cellMatch = e.Item.FindControl("cellMatch") as HtmlTableCell;
                    if (cellMatch != null) cellMatch.InnerText = result.RuleMatched.ToString();

                    if (conditionMatched)
                    {
                        var conditionHeader = e.Item.FindControl("conditionHeader") as HtmlTableRow;
                        if (conditionHeader != null)
                        {
                            conditionHeader.Attributes["data-itemid"] = itemId;
                            conditionHeader.Attributes["class"] = "warning hide condition condition-header";
                        };

                        var conditionRepeater = e.Item.FindControl("conditionRepeater") as Repeater;
                        if (conditionRepeater != null)
                        {
                            conditionRepeater.DataSource = result.ConditionMatchResult.MatchedConditions
                                .Select(
                                    c =>
                                        new Tuple<Tuple<Condition, string>, LogicalGrouping, string>(c,
                                            result.ConditionMatchResult.LogincalGrouping, itemId))
                                .ToList();
                            conditionRepeater.DataBind();
                        }
                    }
                }
            }
        }

        protected void conditionRepeater_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                if (e.Item.DataItem == null)
                    return;

                var conditionResult = e.Item.DataItem as Tuple<Tuple<Condition, string>, LogicalGrouping, string>;
                if (conditionResult != null)
                {

                    var conditionTuple = conditionResult.Item1;
                    var conditionTupleCondition = conditionTuple.Item1;
                    var pattern = conditionTupleCondition.Pattern;
                    var inputString = conditionTupleCondition.InputString;
                    var tokenizedInput = conditionTuple.Item2;
                    var matchType = conditionTupleCondition.CheckIfInputString.ToString();

                    var itemId = conditionResult.Item3;

                    var conditionCellPattern = e.Item.FindControl("conditionPattern") as HtmlTableCell;
                    if (conditionCellPattern != null) conditionCellPattern.InnerText = pattern;

                    var conditionCellInputToken = e.Item.FindControl("conditionInputToken") as HtmlTableCell;
                    if (conditionCellInputToken != null) conditionCellInputToken.InnerText = inputString;

                    var conditionCellInput = e.Item.FindControl("conditionInput") as HtmlTableCell;
                    if (conditionCellInput != null) conditionCellInput.InnerText = tokenizedInput;

                    var conditionMatchType = e.Item.FindControl("conditionMatchType") as HtmlTableCell;
                    if (conditionMatchType != null) conditionMatchType.InnerText = matchType;

                    var conditionRow = e.Item.FindControl("conditionRow") as HtmlTableRow;
                    if (conditionRow != null)
                    {
                        conditionRow.Attributes["class"] = "hide warning condition-item condition";
                        conditionRow.Attributes["data-itemid"] = itemId;
                    }
                }
            }
        }
    }
}