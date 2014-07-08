using System;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;

namespace Hi.UrlRewrite.Templates.Action.RedirectType
{
    public partial class RedirectTypeItem : CustomItem
    {

        public static readonly string TemplateId = "{5E020836-C778-4283-B199-82147C4C122F}";


        #region Boilerplate CustomItem Code

        public RedirectTypeItem(Item innerItem)
            : base(innerItem)
        {

        }

        public static implicit operator RedirectTypeItem(Item innerItem)
        {
            return innerItem != null ? new RedirectTypeItem(innerItem) : null;
        }

        public static implicit operator Item(RedirectTypeItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods


        #endregion //Field Instance Methods
    }
}