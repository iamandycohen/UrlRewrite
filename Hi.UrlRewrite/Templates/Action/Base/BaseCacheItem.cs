using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.UrlRewrite.Templates.Action.Base
{

    public partial class BaseCacheItem : CustomItem
    {

        public static readonly string TemplateId = "{00C9EC91-5A01-4B46-AEE9-A040FCBA7777}";


        #region Boilerplate CustomItem Code

        public BaseCacheItem(Item innerItem)
            : base(innerItem)
        {

        }

        public static implicit operator BaseCacheItem(Item innerItem)
        {
            return innerItem != null ? new BaseCacheItem(innerItem) : null;
        }

        public static implicit operator Item(BaseCacheItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods

        public LookupField HttpCacheability
        {
            get
            {
                return new LookupField(InnerItem.Fields["HttpCacheability"]);
            }
        }


        #endregion //Field Instance Methods
    }

}
