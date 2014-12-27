using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Hi.UrlRewrite.Templates.Match
{
    public partial class MatchTagItem : CustomItem
    {

        public static readonly string TemplateId = "{B88D1713-7511-40D0-B71D-51A5E14C7C7E}";


        private readonly BaseUrlRewriteItem _BaseUrlRewriteItem;
        public BaseUrlRewriteItem BaseUrlRewriteItem { get { return _BaseUrlRewriteItem; } }

        #region Boilerplate CustomItem Code

        public MatchTagItem(Item innerItem)
            : base(innerItem)
        {
            _BaseUrlRewriteItem = new BaseUrlRewriteItem(innerItem);
        }

        public static implicit operator MatchTagItem(Item innerItem)
        {
            return innerItem != null ? new MatchTagItem(innerItem) : null;
        }

        public static implicit operator Item(MatchTagItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods

        public TextField Tag
        {
            get
            {
                return new TextField(InnerItem.Fields["Tag"]);
            }
        }


        public TextField Attribute
        {
            get
            {
                return new TextField(InnerItem.Fields["Attribute"]);
            }
        }

        #endregion //Field Instance Methods
    }
}