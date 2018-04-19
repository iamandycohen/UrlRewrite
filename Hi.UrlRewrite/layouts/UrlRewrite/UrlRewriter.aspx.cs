using Hi.UrlRewrite.Caching;
using Hi.UrlRewrite.Entities.Rules;
using Hi.UrlRewrite.Processing;
using Hi.UrlRewrite.Processing.Results;
using Sitecore.Data;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Hi.UrlRewrite.sitecore_modules.Shell.UrlRewrite
{
    public partial class UrlRewriter : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (new SecurityDisabler())
            {
                var urlRewriter = new InboundRewriter(Request.ServerVariables, Request.Headers);

                var db = Sitecore.Data.Database.GetDatabase(Context.Items["urlrewrite:db"] as string);
                var requestResult = Context.Items["urlrewrite:result"] as ProcessInboundRulesResult;

                if (requestResult != null)
                {
                    //TODO: Curently this only reflects the result of Redirect Actions - make this call to logging reflect all action types
                    Hi.UrlRewrite.Log.Info(this, db, "Redirecting {0} to {1} [{2}]", requestResult.OriginalUri, requestResult.RewrittenUri,
                        requestResult.StatusCode);

                    urlRewriter.ExecuteResult(new HttpContextWrapper(Context), requestResult);
                }
            }
        }

    }
}