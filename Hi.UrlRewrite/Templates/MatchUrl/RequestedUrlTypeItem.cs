using System;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;

namespace Hi.UrlRewrite.Templates.MatchUrl
{
    public partial class RequestedUrlTypeItem : CustomItem
    {

        public static readonly string TemplateId = "{54FB682D-57DA-401F-853A-7556F0213C4C}";


        #region Boilerplate CustomItem Code

        public RequestedUrlTypeItem(Item innerItem)
            : base(innerItem)
        {

        }

        public static implicit operator RequestedUrlTypeItem(Item innerItem)
        {
            return innerItem != null ? new RequestedUrlTypeItem(innerItem) : null;
        }

        public static implicit operator Item(RequestedUrlTypeItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods


        #endregion //Field Instance Methods
    }
}