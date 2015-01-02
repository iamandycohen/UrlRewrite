using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.UrlRewrite.Templates.Action.Base
{

    public partial class BaseRewriteUrlItem : CustomItem
    {

        public static readonly string TemplateId = "{078D5677-484F-4998-85A6-542CE788D840}";


        #region Boilerplate CustomItem Code

        public BaseRewriteUrlItem(Item innerItem)
            : base(innerItem)
        {

        }

        public static implicit operator BaseRewriteUrlItem(Item innerItem)
        {
            return innerItem != null ? new BaseRewriteUrlItem(innerItem) : null;
        }

        public static implicit operator Item(BaseRewriteUrlItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods

        public LinkField RewriteUrl
        {
            get
            {
                return new LinkField(InnerItem.Fields["Rewrite URL"]);
            }
        }


        #endregion //Field Instance Methods
    }

}
