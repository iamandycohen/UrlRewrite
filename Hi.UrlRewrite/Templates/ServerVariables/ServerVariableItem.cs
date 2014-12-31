using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hi.UrlRewrite.Templates.ServerVariables
{
    public class ServerVariableItem : CustomItem
    {
        public static readonly string TemplateId = "{E073CD4C-2747-433D-B6C0-3C52BA953C97}";

        #region Inherited Base Templates

        private readonly BaseServerVariableItem _BaseServerVariableItem;
        public BaseServerVariableItem BaseServerVariableItem { get { return _BaseServerVariableItem; } }

        #endregion

        public ServerVariableItem(Item innerItem)
            : base(innerItem)
        {
            _BaseServerVariableItem = new BaseServerVariableItem(innerItem);
        }

        public static implicit operator ServerVariableItem(Item innerItem)
        {
            return innerItem != null ? new ServerVariableItem(innerItem) : null;
        }

        public static implicit operator Item(ServerVariableItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }


    }
}