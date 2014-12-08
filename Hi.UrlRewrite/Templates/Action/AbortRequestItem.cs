using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.UrlRewrite.Templates.Action.Base;
using Sitecore.Data.Items;

namespace Hi.UrlRewrite.Templates.Action
{
    public partial class AbortRequestItem : CustomItem
    {

        public static readonly string TemplateId = "{BD8E6E5E-62F8-4397-99CA-B0502AFD14B9}";

        #region Inherited Base Templates

        private readonly BaseActionItem _BaseAction;
        public BaseActionItem BaseAction { get { return _BaseAction; } }

        #endregion

        #region Boilerplate CustomItem Code

        public AbortRequestItem(Item innerItem)
            : base(innerItem)
        {
            _BaseAction = new BaseActionItem(innerItem);
        }

        public static implicit operator AbortRequestItem(Item innerItem)
        {
            return innerItem != null ? new AbortRequestItem(innerItem) : null;
        }

        public static implicit operator Item(AbortRequestItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code
    }
}
