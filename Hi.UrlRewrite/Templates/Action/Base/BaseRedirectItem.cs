using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hi.UrlRewrite.Templates.Action.Base
{
    public class BaseRedirectItem : CustomItem
    {
        public static readonly string TemplateId = "{D28318B2-5793-4ABA-BFB3-3C3FBC00AA92}";

        #region Inherited Base Templates

        private readonly BaseStopProcessingItem _BaseStopProcessingItem;
        public BaseStopProcessingItem BaseStopProcessingItem { get { return _BaseStopProcessingItem; } }

        private readonly BaseAppendQuerystringItem _BaseAppendQuerystringItem;
        public BaseAppendQuerystringItem BaseAppendQuerystringItem { get { return _BaseAppendQuerystringItem; } }

        private readonly BaseRewriteUrlItem _BaseRewriteUrlItem;
        public BaseRewriteUrlItem BaseRewriteUrlItem { get { return _BaseRewriteUrlItem; } }

        private readonly BaseRedirectTypeItem _BaseRedirectTypeItem;
        public BaseRedirectTypeItem BaseRedirectTypeItem { get { return _BaseRedirectTypeItem; } }

        private readonly BaseCacheItem _BaseCacheItem;
        public BaseCacheItem BaseCacheItem { get { return _BaseCacheItem; } }

        #endregion

        #region Boilerplate CustomItem Code

        public BaseRedirectItem(Item innerItem)
            : base(innerItem)
        {
            _BaseStopProcessingItem = new BaseStopProcessingItem(innerItem);
            _BaseAppendQuerystringItem = new BaseAppendQuerystringItem(innerItem);
            _BaseRewriteUrlItem = new BaseRewriteUrlItem(innerItem);
            _BaseRedirectTypeItem = new BaseRedirectTypeItem(innerItem);
            _BaseCacheItem = new BaseCacheItem(innerItem);
        }

        public static implicit operator BaseRedirectItem(Item innerItem)
        {
            return innerItem != null ? new BaseRedirectItem(innerItem) : null;
        }

        public static implicit operator Item(BaseRedirectItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods

        #endregion //Field Instance Methods
    }
}