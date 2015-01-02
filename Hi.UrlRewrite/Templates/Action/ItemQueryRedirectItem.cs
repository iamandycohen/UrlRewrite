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

        private readonly BaseAppendQuerystringItem _BaseAppendQuerystringItem;
        public BaseAppendQuerystringItem BaseAppendQuerystringItem { get { return _BaseAppendQuerystringItem; } }

        private readonly BaseCacheItem _BaseCacheItem;
        public BaseCacheItem BaseCacheItem { get { return _BaseCacheItem; } }

        private readonly BaseRedirectTypeItem _BaseRedirectTypeItem;
        public BaseRedirectTypeItem BaseRedirectTypeItem { get { return _BaseRedirectTypeItem; } }

        private readonly BaseStopProcessingItem _BaseStopProcessingItem;
        public BaseStopProcessingItem BaseStopProcessingItem { get { return _BaseStopProcessingItem; } }
        #endregion

        #region Boilerplate CustomItem Code

        public ItemQueryRedirectItem(Item innerItem)
            : base(innerItem)
        {
            _BaseAppendQuerystringItem = new BaseAppendQuerystringItem(innerItem);
            _BaseCacheItem = new BaseCacheItem(innerItem);
            _BaseRedirectTypeItem = new BaseRedirectTypeItem(innerItem);
            _BaseStopProcessingItem = new BaseStopProcessingItem(innerItem);
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