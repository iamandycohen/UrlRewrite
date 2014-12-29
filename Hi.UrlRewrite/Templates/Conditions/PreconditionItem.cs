using System;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;

namespace Hi.UrlRewrite.Templates.Conditions
{
    public partial class PreconditionItem : CustomItem
    {

        public static readonly string TemplateId = "{A83E69AE-B93F-45C1-A20B-E6879BD04598}";

        #region Inherited Base Templates

        private readonly BaseUrlRewriteItem _BaseUrlRewriteItem;
        public BaseUrlRewriteItem BaseUrlRewriteItem { get { return _BaseUrlRewriteItem; } }

        private readonly PreconditionUsingItem _PreconditionUsingItem;
        public PreconditionUsingItem PreconditionUsingItem { get { return _PreconditionUsingItem; } }

        private readonly ConditionLogicalGroupingItem _ConditionLogicalGroupingItem;
        public ConditionLogicalGroupingItem ConditionLogicalGroupingItem { get { return _ConditionLogicalGroupingItem; } }

        #endregion

        #region Boilerplate CustomItem Code

        public PreconditionItem(Item innerItem)
            : base(innerItem)
        {
            _BaseUrlRewriteItem = new BaseUrlRewriteItem(innerItem);
            _PreconditionUsingItem = new PreconditionUsingItem(innerItem);
            _ConditionLogicalGroupingItem = new ConditionLogicalGroupingItem(innerItem);
        }

        public static implicit operator PreconditionItem(Item innerItem)
        {
            return innerItem != null ? new PreconditionItem(innerItem) : null;
        }

        public static implicit operator Item(PreconditionItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods

        #endregion //Field Instance Methods
    }
}