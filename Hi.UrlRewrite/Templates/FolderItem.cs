using System;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;

namespace Hi.UrlRewrite.Templates
{
    public partial class SettingsItem : CustomItem
    {

        public static readonly string TemplateId = "{B3A4B170-59DE-4438-B4E8-FE74A3C24C00}";


        #region Boilerplate CustomItem Code

        public SettingsItem(Item innerItem)
            : base(innerItem)
        {

        }

        public static implicit operator SettingsItem(Item innerItem)
        {
            return innerItem != null ? new SettingsItem(innerItem) : null;
        }

        public static implicit operator Item(SettingsItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods

        public MultilistField InstallationPublishingTargets
        {
	        get
	        {
		        return new MultilistField(InnerItem.Fields["Installation Publishing Targets"]);
	        }
        }

        #endregion //Field Instance Methods
    }
}