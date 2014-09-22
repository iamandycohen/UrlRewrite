using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.UrlRewrite.Templates.Action.HttpCacheabilityType
{

    public partial class HttpCacheabilityTypeItem : CustomItem
    {

        public static readonly string TemplateId = "{482F740A-FEF3-4C88-AE89-3D5859FD0D6D}";


        #region Boilerplate CustomItem Code

        public HttpCacheabilityTypeItem(Item innerItem)
            : base(innerItem)
        {

        }

        public static implicit operator HttpCacheabilityTypeItem(Item innerItem)
        {
            return innerItem != null ? new HttpCacheabilityTypeItem(innerItem) : null;
        }

        public static implicit operator Item(HttpCacheabilityTypeItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods


        #endregion //Field Instance Methods
    }

}
