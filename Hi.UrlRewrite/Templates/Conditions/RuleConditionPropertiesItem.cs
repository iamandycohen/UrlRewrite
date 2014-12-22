using System;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;

namespace Hi.UrlRewrite.Templates.Conditions
{
    public partial class RuleConditionPropertiesItem : CustomItem
    {

        public static readonly string TemplateId = "{1652DCC0-6319-43D4-853F-B5A441866F86}";


        #region Boilerplate CustomItem Code

        public RuleConditionPropertiesItem(Item innerItem)
            : base(innerItem)
        {

        }

        public static implicit operator RuleConditionPropertiesItem(Item innerItem)
        {
            return innerItem != null ? new RuleConditionPropertiesItem(innerItem) : null;
        }

        public static implicit operator Item(RuleConditionPropertiesItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods

        public LookupField LogicalGrouping
        {
            get
            {
                return new LookupField(InnerItem.Fields["Logical grouping"]);
            }
        }

        #endregion //Field Instance Methods
    }
}