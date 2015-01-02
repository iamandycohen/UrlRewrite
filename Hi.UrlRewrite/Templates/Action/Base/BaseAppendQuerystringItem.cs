using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.UrlRewrite.Templates.Action.Base
{

    public partial class BaseAppendQuerystringItem : CustomItem
    {

        public static readonly string TemplateId = "{DA9B513D-D4FA-4FC3-A49D-EB6754C37DD1}";


        #region Boilerplate CustomItem Code

        public BaseAppendQuerystringItem(Item innerItem)
            : base(innerItem)
        {

        }

        public static implicit operator BaseAppendQuerystringItem(Item innerItem)
        {
            return innerItem != null ? new BaseAppendQuerystringItem(innerItem) : null;
        }

        public static implicit operator Item(BaseAppendQuerystringItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods

        public CheckboxField AppendQueryString
        {
            get
            {
                return new CheckboxField(InnerItem.Fields["Append query string"]);
            }
        }


        #endregion //Field Instance Methods
    }

}
