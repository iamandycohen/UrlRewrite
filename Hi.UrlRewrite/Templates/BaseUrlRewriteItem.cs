using System;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;

namespace Hi.UrlRewrite.Templates
{
    public partial class BaseUrlRewriteItem : CustomItem
    {

        public static readonly string TemplateId = "{83AFF04D-C0DA-44D4-8A7E-4BC0A89903E8}";


        #region Boilerplate CustomItem Code

        public BaseUrlRewriteItem(Item innerItem)
            : base(innerItem)
        {

        }

        public static implicit operator BaseUrlRewriteItem(Item innerItem)
        {
            return innerItem != null ? new BaseUrlRewriteItem(innerItem) : null;
        }

        public static implicit operator Item(BaseUrlRewriteItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods


        #endregion //Field Instance Methods
    }
}