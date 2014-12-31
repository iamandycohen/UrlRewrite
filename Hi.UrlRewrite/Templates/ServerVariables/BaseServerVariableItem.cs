using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hi.UrlRewrite.Templates.ServerVariables
{
    public class BaseServerVariableItem : CustomItem
    {
        public static readonly string TemplateId = "{ED218F0C-2AF5-4D0A-AA45-E089F9862E0C}";

        #region Inherited Base Templates

        #endregion

        public BaseServerVariableItem(Item innerItem)
            : base(innerItem)
        {

        }

        public static implicit operator BaseServerVariableItem(Item innerItem)
        {
            return innerItem != null ? new BaseServerVariableItem(innerItem) : null;
        }

        public static implicit operator Item(BaseServerVariableItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        public TextField VariableName
        {
            get
            {
                return new TextField(InnerItem.Fields["Variable Name"]);
            }
        }

        public TextField Value
        {
            get
            {
                return new TextField(InnerItem.Fields["Value"]);
            }
        }

        public CheckboxField ReplaceExistingValue
        {
            get
            {
                return new CheckboxField(InnerItem.Fields["Replace existing value"]);
            }
        }

    }

}