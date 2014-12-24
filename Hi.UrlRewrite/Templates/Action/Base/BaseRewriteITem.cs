using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.UrlRewrite.Templates.Action.Base
{

    public partial class BaseRewriteItem : CustomItem
    {

        public static readonly string TemplateId = "{7E95E23F-437C-46DC-97AA-F2F6C79B78C1}";

        #region Inherited Base Templates

        private readonly BaseStopProcessingActionItem _BaseStopProcessingActionItem;
        public BaseStopProcessingActionItem BaseStopProcessingActionItem { get { return _BaseStopProcessingActionItem; } }

        private readonly BaseCacheItem _BaseCacheItem;
        public BaseCacheItem BaseCacheItem { get { return _BaseCacheItem; } }

        private readonly BaseRedirectTypeItem _BaseRedirectTypeItem;
        public BaseRedirectTypeItem BaseRedirectTypeItem { get { return _BaseRedirectTypeItem; } }

        #endregion

        #region Boilerplate CustomItem Code

        public BaseRewriteItem(Item innerItem)
            : base(innerItem)
        {
            _BaseStopProcessingActionItem = new BaseStopProcessingActionItem(innerItem);
            _BaseCacheItem = new BaseCacheItem(innerItem);
            _BaseRedirectTypeItem = new BaseRedirectTypeItem(innerItem);
        }

        public static implicit operator BaseRewriteItem(Item innerItem)
        {
            return innerItem != null ? new BaseRewriteItem(innerItem) : null;
        }

        public static implicit operator Item(BaseRewriteItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods

        #endregion //Field Instance Methods
    }

}
