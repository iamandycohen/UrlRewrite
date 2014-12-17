using System;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;

namespace Hi.UrlRewrite.Templates.Action.Base
{
    public partial class BaseRedirectTypeItem : CustomItem
    {

        public static readonly string TemplateId = "{2372E073-6219-4F31-A7B0-DDA295636A6A}s";

        #region Boilerplate CustomItem Code

        public BaseRedirectTypeItem(Item innerItem)
            : base(innerItem)
        {
        }

        public static implicit operator BaseRedirectTypeItem(Item innerItem)
        {
            return innerItem != null ? new BaseRedirectTypeItem(innerItem) : null;
        }

        public static implicit operator Item(BaseRedirectTypeItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code

        #region Field Instance Methods

        public LookupField RedirectType
        {
            get
            {
                return new LookupField(InnerItem.Fields["Redirect type"]);
            }
        }


        #endregion //Field Instance Methods
    }
}