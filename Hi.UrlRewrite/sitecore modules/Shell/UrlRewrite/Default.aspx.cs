using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Hi.UrlRewrite.Processing;
using Hi.UrlRewrite.Processing.Results;
using Sitecore.Data;
using Sitecore.Sites;

namespace Hi.UrlRewrite.sitecore_modules.Shell.UrlRewrite
{
    public partial class Default : System.Web.UI.Page
    {
        private Database _db;
        private List<RuleResult> _processedResults;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
            else
            {
                var rewriter = new UrlRewriter();
                _db = Sitecore.Context.ContentDatabase;

                var inboundRules = RulesEngine.GetInboundRules(_db);
                ProcessRequestResult results;

                var requestUri = new Uri(txtUrl.Text);
                var siteContext = SiteContextFactory.GetSiteContext(requestUri.Host, requestUri.AbsolutePath, requestUri.Port);

                using (new SiteContextSwitcher(siteContext))
                using (new DatabaseSwitcher(_db))
                {
                    results = rewriter.ProcessRequestUrl(new Uri(txtUrl.Text), inboundRules);

                    _processedResults = results.ProcessedResults;
                }


                resultsRepeater.DataSource = _processedResults;
                resultsRepeater.DataBind();

                txtFinalUrl.InnerText = results.RewrittenUri.ToString();

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

                    if (result.RuleMatched)
                    {
                        var tableRow = e.Item.FindControl("tableRow") as HtmlTableRow;
                        tableRow.Attributes["class"] = "matched";
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
                }
            }
        }
    }
}