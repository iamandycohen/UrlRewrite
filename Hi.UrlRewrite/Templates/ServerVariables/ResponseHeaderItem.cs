using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hi.UrlRewrite.Templates.ServerVariables
{
    public class ResponseHeaderItem : CustomItem
    {
        public static readonly string TemplateId = "{BA67D115-9A49-4EAB-8CDF-596CB6F0CBF1}";

        #region Inherited Base Templates

        private readonly BaseServerVariableItem _BaseServerVariableItem;
        public BaseServerVariableItem BaseServerVariableItem { get { return _BaseServerVariableItem; } }

        #endregion

        public ResponseHeaderItem(Item innerItem)
            : base(innerItem)
        {
            _BaseServerVariableItem = new BaseServerVariableItem(innerItem);
        }

        public static implicit operator ResponseHeaderItem(Item innerItem)
        {
            return innerItem != null ? new ResponseHeaderItem(innerItem) : null;
        }

        public static implicit operator Item(ResponseHeaderItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }


    }
}