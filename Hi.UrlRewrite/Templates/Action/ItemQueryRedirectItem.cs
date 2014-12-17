using System;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;

namespace Hi.UrlRewrite.Templates.Action.Base
{
    public partial class ItemQueryRedirectItem : CustomItem
    {

        public static readonly string TemplateId = "{5B7FB661-CB6C-449C-9C84-2672538AC77C}";

        #region Inherited Base Templates

        private readonly BaseRewriteItem _BaseRewriteItem;
        public BaseRewriteItem BaseRewriteItem { get { return _BaseRewriteItem; } }

        #endregion

        #region Boilerplate CustomItem Code

        public ItemQueryRedirectItem(Item innerItem)
            : base(innerItem)
        {
            _BaseRewriteItem = new BaseRewriteItem(innerItem);
        }

        public static implicit operator ItemQueryRedirectItem(Item innerItem)
        {
            return innerItem != null ? new ItemQueryRedirectItem(innerItem) : null;
        }

        public static implicit operator Item(ItemQueryRedirectItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods


        public TextField ItemQuery
        {
            get
            {
                return new TextField(InnerItem.Fields["Item Query"]);
            }
        }

        #endregion //Field Instance Methods
    }
}