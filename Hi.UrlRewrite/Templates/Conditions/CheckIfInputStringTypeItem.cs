using System;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;

namespace Hi.UrlRewrite.Templates.Conditions
{
    public partial class CheckIfInputStringTypeItem : CustomItem
    {

        public static readonly string TemplateId = "{33B5817B-9BE3-4A97-8344-9096FEF5B8BC}";


        #region Boilerplate CustomItem Code

        public CheckIfInputStringTypeItem(Item innerItem)
            : base(innerItem)
        {

        }

        public static implicit operator CheckIfInputStringTypeItem(Item innerItem)
        {
            return innerItem != null ? new CheckIfInputStringTypeItem(innerItem) : null;
        }

        public static implicit operator Item(CheckIfInputStringTypeItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods


        #endregion //Field Instance Methods
    }
}