using Hi.UrlRewrite.Templates.Action.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Hi.UrlRewrite.Templates.Action
{
    public class CustomResponseItem : CustomItem
    {
        public static readonly string TemplateId = "{A96D8E98-38B7-4D99-BE55-D5225F6D8279}";

        #region Inherited Base Templates

        private readonly BaseActionItem _BaseAction;
        public BaseActionItem BaseAction { get { return _BaseAction; } }

        #endregion

        #region Boilerplate CustomItem Code

        public CustomResponseItem(Item innerItem)
            : base(innerItem)
        {
            _BaseAction = new BaseActionItem(innerItem);
        }

        public static implicit operator CustomResponseItem(Item innerItem)
        {
            return innerItem != null ? new CustomResponseItem(innerItem) : null;
        }

        public static implicit operator Item(CustomResponseItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code

        #region Field Instance Methods

        public TextField StatusCode
        {
            get
            {
                return new TextField(InnerItem.Fields["Status code"]);
            }
        }

        public TextField SubstatusCode
        {
            get
            {
                return new TextField(InnerItem.Fields["Substatus code"]);
            }
        }

        public TextField Reason
        {
            get
            {
                return new TextField(InnerItem.Fields["Reason"]);
            }
        }

        public TextField ErrorDescription
        {
            get
            {
                return new TextField(InnerItem.Fields["Error description"]);
            }
        }

        #endregion

    }
}
