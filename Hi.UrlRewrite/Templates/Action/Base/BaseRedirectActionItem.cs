using System;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;

namespace Hi.UrlRewrite.Templates.Action.Base
{
    public partial class BaseRedirectActionItem : CustomItem
    {

        public static readonly string TemplateId = "{D28318B2-5793-4ABA-BFB3-3C3FBC00AA92}";

        #region Inherited Base Templates

        private readonly BaseStopProcessingActionItem _BaseStopProcessingAction;
        public BaseStopProcessingActionItem BaseStopProcessingAction { get { return _BaseStopProcessingAction; } }

        private readonly BaseCacheItem _BaseCacheItem;
        public BaseCacheItem BaseCacheItem { get { return _BaseCacheItem; } }

        #endregion

        #region Boilerplate CustomItem Code

        public BaseRedirectActionItem(Item innerItem)
            : base(innerItem)
        {
            _BaseStopProcessingAction = new BaseStopProcessingActionItem(innerItem);
            _BaseCacheItem = new BaseCacheItem(innerItem);
        }

        public static implicit operator BaseRedirectActionItem(Item innerItem)
        {
            return innerItem != null ? new BaseRedirectActionItem(innerItem) : null;
        }

        public static implicit operator Item(BaseRedirectActionItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods


        public LinkField RewriteUrl
        {
            get
            {
                return new LinkField(InnerItem.Fields["Rewrite URL"]);
            }
        }


        public CheckboxField AppendQueryString
        {
            get
            {
                return new CheckboxField(InnerItem.Fields["Append query string"]);
            }
        }


        #endregion //Field Instance Methods
    }
}