using System;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;

namespace Hi.UrlRewrite.Templates.Conditions
{
    public partial class ConditionAdvancedItem : CustomItem
    {

        public static readonly string TemplateId = "{0035B824-507B-4F69-9A1E-FFC2DD738D2D}";

        #region Inherited Base Templates

        private readonly BaseConditionItem _BaseConditionItem;
        public BaseConditionItem BaseConditionItem { get { return _BaseConditionItem; } }

        #endregion

        #region Boilerplate CustomItem Code

        public ConditionAdvancedItem(Item innerItem)
            : base(innerItem)
        {
            _BaseConditionItem = new BaseConditionItem(innerItem);
        }

        public static implicit operator ConditionAdvancedItem(Item innerItem)
        {
            return innerItem != null ? new ConditionAdvancedItem(innerItem) : null;
        }

        public static implicit operator Item(ConditionAdvancedItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods


        public TextField ConditionInputString
        {
            get
            {
                return new TextField(InnerItem.Fields["Condition Input String"]);
            }
        }

        #endregion //Field Instance Methods
    }
}