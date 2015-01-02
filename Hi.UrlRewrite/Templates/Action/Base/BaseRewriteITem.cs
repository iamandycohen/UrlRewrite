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

        private readonly BaseStopProcessingItem _BaseStopProcessingActionItem;
        public BaseStopProcessingItem BaseStopProcessingActionItem { get { return _BaseStopProcessingActionItem; } }

        private readonly BaseAppendQuerystringItem _BaseAppendQuerystringItem;
        public BaseAppendQuerystringItem BaseAppendQuerystringItem { get { return _BaseAppendQuerystringItem; } }

        private readonly BaseRewriteUrlItem _BaseRewriteUrlItem;
        public BaseRewriteUrlItem BaseRewriteUrlItem { get { return _BaseRewriteUrlItem; } }

        #endregion

        #region Boilerplate CustomItem Code

        public BaseRewriteItem(Item innerItem)
            : base(innerItem)
        {
            _BaseStopProcessingActionItem = new BaseStopProcessingItem(innerItem);
            _BaseAppendQuerystringItem = new BaseAppendQuerystringItem(innerItem);
            _BaseRewriteUrlItem = new BaseRewriteUrlItem(innerItem);
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
