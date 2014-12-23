using System;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;

namespace Hi.UrlRewrite.Templates
{
    public partial class FolderItem : CustomItem
    {

        public static readonly string TemplateId = "{A87A00B1-E6DB-45AB-8B54-636FEC3B5523}";


        #region Boilerplate CustomItem Code

        public FolderItem(Item innerItem)
            : base(innerItem)
        {

        }

        public static implicit operator FolderItem(Item innerItem)
        {
            return innerItem != null ? new FolderItem(innerItem) : null;
        }

        public static implicit operator Item(FolderItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods


        #endregion //Field Instance Methods
    }
}