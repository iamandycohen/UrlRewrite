using System;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;

namespace Hi.UrlRewrite.Templates.Conditions
{
    public partial class LogicalGroupingTypeItem : CustomItem
    {

        public static readonly string TemplateId = "{075E8F5E-04A4-404D-A780-939148E7FB45}";


        #region Boilerplate CustomItem Code

        public LogicalGroupingTypeItem(Item innerItem)
            : base(innerItem)
        {

        }

        public static implicit operator LogicalGroupingTypeItem(Item innerItem)
        {
            return innerItem != null ? new LogicalGroupingTypeItem(innerItem) : null;
        }

        public static implicit operator Item(LogicalGroupingTypeItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods


        #endregion //Field Instance Methods
    }
}