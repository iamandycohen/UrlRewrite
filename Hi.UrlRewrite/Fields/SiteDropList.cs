using System.Web.UI;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Sites;
using System;
using System.Linq;
using Control = Sitecore.Web.UI.HtmlControls.Control;

namespace Hi.UrlRewrite.Fields
{
    public class SiteDropList : Control
    {

        public SiteDropList()
        {
            SetClass();
            Activation = true;
        }

        protected void SetClass()
        {
            Class = "scContentControl";
        }

        protected override void DoRender(HtmlTextWriter output)
        {
            string err = null;
            output.Write("<select" + GetControlAttributes() + ">");
            output.Write("<option value=\"\"></option>");

            bool valueFound = String.IsNullOrEmpty(Value);

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

                valueFound = valueFound || value == Value;

                string selected = Value == value ? " selected=\"selected\"" : string.Empty;

                output.Write(@"<option value=""{0}"" {1}>{2}</option>", value, selected, title);
            }

            if (!valueFound)
            {
                err = Translate.Text("Value not in the selection list.");
            }

            if (err != null)
            {
                output.Write("<optgroup label=\"{0}\">", err);
                output.Write("<option value=\"{0}\" selected=\"selected\">{1}</option>", Value, Value);
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
            HasPostData = true;

            if (value == null)
            {
                return false;
            }

            Log.Info(this + " : Field : " + GetViewStateString("Field"), this);
            Log.Info(this + " : FieldName : " + GetViewStateString("FieldName"), this);

            if (GetViewStateString("Value") != value)
            {
                SetModified();
            }

            SetViewStateString("Value", value);
            return true;
        }

        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");
            base.OnLoad(e);

            if (!HasPostData)
            {
                LoadPostData(string.Empty);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");
            base.OnPreRender(e);
            ServerProperties["Value"] = ServerProperties["Value"];
        }

        private static void SetModified()
        {
            Sitecore.Context.ClientPage.Modified = true;
        }

        public bool HasPostData { get; set; }
    }
}
