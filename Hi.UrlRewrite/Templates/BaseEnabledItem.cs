using System;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;

namespace Hi.UrlRewrite.Templates
{
    public partial class BaseEnabledItem : CustomItem
    {

        public static readonly string TemplateId = "{9CE37445-8572-4244-87E7-B0AAFBF87A35}";


        #region Boilerplate CustomItem Code

        public BaseEnabledItem(Item innerItem)
            : base(innerItem)
        {

        }

        public static implicit operator BaseEnabledItem(Item innerItem)
        {
            return innerItem != null ? new BaseEnabledItem(innerItem) : null;
        }

        public static implicit operator Item(BaseEnabledItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods

        public CheckboxField Enabled
        {
            get
            {
                return new CheckboxField(InnerItem.Fields["Enabled"]);
            }
        }

        #endregion //Field Instance Methods
    }
}