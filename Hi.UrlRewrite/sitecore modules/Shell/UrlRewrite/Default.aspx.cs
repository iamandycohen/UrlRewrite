using Hi.UrlRewrite.Entities.Actions;
using Hi.UrlRewrite.Entities.Conditions;
using Hi.UrlRewrite.Processing;
using Hi.UrlRewrite.Processing.Results;
using Sitecore.Configuration;
using Sitecore.Data;
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hi.UrlRewrite.sitecore_modules.Shell.UrlRewrite
{
    public partial class Default : System.Web.UI.Page
    {
        private Database _db;

        protected void Page_Load(object sender, EventArgs e)
        {
            GetAndSetCurrentDatabase();
            CreateDatabaseDropdown();

            if (!IsPostBack)
            {

            }
            else
            {

                try
                {

                    Page.Validate();

                    if (Page.IsValid)
                    {
                        CreateRewriteForm();
                    }
                    else
                    {
                        if (!vldTxtUrl.IsValid)
                        {
                            divFormGroup.Attributes["class"] = "form-group has-error";
                        }
                    }
                }
                catch (Exception ex)
                {
                    DisplayException(ex);
                }

            }
        }

        private void CreateDatabaseDropdown()
        {
            litCurrentDatabase.Text = _db.Name;

            var databases = Factory.GetDatabases()
                .Where(db => db.HasContentItem);

            rptDatabase.DataSource = databases;
            rptDatabase.DataBind();
        }

        private void GetAndSetCurrentDatabase()
        {
            var currentDatabase = Request.QueryString["db"];
            if (!string.IsNullOrEmpty(currentDatabase))
            {
                _db = Factory.GetDatabase(currentDatabase);
            }
            else
            {
                _db = Sitecore.Context.ContentDatabase;
            }
        }

        private void DisplayException(Exception ex)
        {
            divTable.Visible = false;
            divInfo.Visible = false;
            divError.Visible = true;
            txtError.InnerText = @"Exception: 

    " + ex.Message;
        }

        private void CreateRewriteForm()
        {
            divFormGroup.Attributes["class"] = "form-group";
            divTable.Visible = true;

            var inboundRulesHelper = new InboundRulesHelper();

            // TODO: allow variables to be set in the UI

            var results = inboundRulesHelper.GetUrlResults(txtUrl.Text, _db);

            if (results == null)
            {
                divTable.Visible = false;
                divInfo.Visible = true;
                divInfo.InnerText = "Sorry, I couldn't find any rules to process. :(";
            }
            else
            {
                resultsRepeater.DataSource = results.ProcessedResults;
                resultsRepeater.DataBind();

                var isAbort = results.FinalAction is AbortRequest;
                var isCustomResponse = results.FinalAction is CustomResponse;

                if (isAbort)
                {
                    txtFinalUrl.InnerText = "Aborted";
                }
                else if (isCustomResponse)
                {
                    var customResponse = results.FinalAction as CustomResponse;
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
        }

        protected void resultsRepeater_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                if (e.Item.DataItem == null)
                    return;

                var result = e.Item.DataItem as InboundRuleResult;
                if (result != null)
                {
                    var ruleMatched = result.RuleMatched;
                    var isAbort = ruleMatched && result.ResultAction is AbortRequest;
                    var isCustomResponse = ruleMatched && result.ResultAction is CustomResponse;
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
                                        new Tuple<MatchedCondition, LogicalGrouping, string>(c,
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

                var conditionResult = e.Item.DataItem as Tuple<MatchedCondition, LogicalGrouping, string>;
                if (conditionResult != null)
                {

                    var conditionTuple = conditionResult.Item1;
                    var conditionTupleCondition = conditionTuple.Condition;
                    var pattern = conditionTupleCondition.Pattern;
                    var inputString = conditionTupleCondition.InputString;
                    var tokenizedInput = conditionTuple.ConditionInput;
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

        protected void rptDatabase_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item) return;

            if (e.Item.DataItem == null)
                return;

            var database = e.Item.DataItem as Database;
            if (database == null) return;

            var link = e.Item.FindControl("lnkDatabase") as HtmlAnchor;
            if (link == null) return;

            link.InnerText = database.Name;
            link.HRef = "?db=" + database.Name;
        }
    }
}