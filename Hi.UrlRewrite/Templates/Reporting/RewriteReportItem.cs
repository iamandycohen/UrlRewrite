using Hi.UrlRewrite.Templates.Conditions;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Hi.UrlRewrite.Templates.Reporting
{
    public class RewriteReportItem : CustomItem
    {
        public static readonly string TemplateId = "{F5CEB67D-1CE8-48EF-8D2E-39F44A21B52E}";

        #region Inherited Base Templates

        #endregion

        public RewriteReportItem(Item innerItem)
            : base(innerItem)
        {
        }

        public static implicit operator RewriteReportItem(Item innerItem)
        {
            return innerItem != null ? new RewriteReportItem(innerItem) : null;
        }

        public static implicit operator Item(RewriteReportItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        public TextField OriginalUrl
        {
            get
            {
                return new TextField(InnerItem.Fields["Original Url"]);
            }
        }

        public TextField RewrittenUrl
        {
            get
            {
                return new TextField(InnerItem.Fields["Rewritten Url"]);
            }
        }

        public TextField DatabaseName
        {
            get
            {
                return new TextField(InnerItem.Fields["Database Name"]);
            }
        }

        public DateField RewriteDate
        {
            get
            {
                return new DateField(InnerItem.Fields["Rewrite Date"]);
            }
        }

        public LookupField Rule
        {
            get
            {
                return new LookupField(InnerItem.Fields["Rule"]);
            }
        }

    }
}
