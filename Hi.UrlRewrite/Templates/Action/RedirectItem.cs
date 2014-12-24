using System;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;
using Hi.UrlRewrite.Templates.Action.Base;

namespace Hi.UrlRewrite.Templates.Action
{
    public partial class RedirectItem : CustomItem
    {

        public static readonly string TemplateId = "{D199EF8B-9D4D-420F-A283-E16D7B575625}";

        #region Inherited Base Templates

        private readonly BaseRedirectActionItem _BaseRedirectActionItem;
        public BaseRedirectActionItem BaseRedirectActionItem { get { return _BaseRedirectActionItem; } }

        #endregion

        #region Boilerplate CustomItem Code

        public RedirectItem(Item innerItem)
            : base(innerItem)
        {
            _BaseRedirectActionItem = new BaseRedirectActionItem(innerItem);

        }

        public static implicit operator RedirectItem(Item innerItem)
        {
            return innerItem != null ? new RedirectItem(innerItem) : null;
        }

        public static implicit operator Item(RedirectItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code

        #region Field Instance Methods

        #endregion //Field Instance Methods
    }
}