using System.Web.UI;
using Sitecore.Data.Items;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Sites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Control = Sitecore.Web.UI.HtmlControls.Control;

namespace Hi.UrlRewrite.Fields
{
    public class SiteDropList : Control
    {
        public SiteDropList()
        {
            this.Class = "scContentControl";
            this.Activation = true;
        }

        protected override void DoRender(HtmlTextWriter output)
        {
            string err = null;
            output.Write("<select" + this.GetControlAttributes() + ">");
            output.Write("<option value=\"\"></option>");

            bool valueFound = String.IsNullOrEmpty(this.Value);

            var sites = SiteManager.GetSites()
                .Where(s =>
                {
                    var rootPath = s.Properties["rootPath"];
                    var db = s.Properties["database"];

                    return (rootPath != null && rootPath.StartsWith(@"/sitecore/content")) && (db != null && !db.Equals("core", StringComparison.InvariantCultureIgnoreCase));
                })
                .ToList();

            foreach (var site in sites)
            {
                string title = site.Name;
                string value = site.Name;

                valueFound = valueFound || value == this.Value;

                string selected = this.Value == value ? " selected=\"selected\"" : string.Empty;

                output.Write(string.Format(@"<option value=""{0}"" {1}>{2}</option>", value, selected, title));
            }

            if (!valueFound)
            {
                err = Sitecore.Globalization.Translate.Text("Value not in the selection list.");
            }

            if (err != null)
            {
                output.Write("<optgroup label=\"" + err + "\">");
                output.Write("<option value=\"" + this.Value + "\" selected=\"selected\">" + this.Value + "</option>");
                output.Write("</optgroup>");
            }

            output.Write("</select>");

            if (err != null)
            {
                output.Write("<div style=\"color:#999999;padding:2px 0px 0px 0px\">{0}</div>", err);
            }

        }

        protected override bool LoadPostData(string value)
        {
            this.HasPostData = true;

            if (value == null)
            {
                return false;
            }

            Sitecore.Diagnostics.Log.Info(this + " : Field : " + this.GetViewStateString("Field"), this);
            Sitecore.Diagnostics.Log.Info(this + " : FieldName : " + this.GetViewStateString("FieldName"), this);

            if (this.GetViewStateString("Value") != value)
            {
                SetModified();
            }

            this.SetViewStateString("Value", value);
            return true;
        }

        protected override void OnLoad(EventArgs e)
        {
            Sitecore.Diagnostics.Assert.ArgumentNotNull(e, "e");
            base.OnLoad(e);

            if (!this.HasPostData)
            {
                this.LoadPostData(string.Empty);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            Sitecore.Diagnostics.Assert.ArgumentNotNull(e, "e");
            base.OnPreRender(e);
            this.ServerProperties["Value"] = this.ServerProperties["Value"];
        }

        private static void SetModified()
        {
            Sitecore.Context.ClientPage.Modified = true;
        }

        public bool HasPostData { get; set; }
    }
}
