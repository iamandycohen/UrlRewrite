using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.UrlRewrite.Templates.Action.Base;
using Sitecore.Data.Items;

namespace Hi.UrlRewrite.Templates.Action
{
    public partial class NoneItem : CustomItem
    {

        public static readonly string TemplateId = "{12FA1A86-77CC-4097-86DB-A66849AF157A}";

        #region Inherited Base Templates

        private readonly BaseStopProcessingItem _BaseStopProcessingActionItem;
        public BaseStopProcessingItem BaseStopProcessingActionItem { get { return _BaseStopProcessingActionItem; } }

        #endregion

        #region Boilerplate CustomItem Code

        public NoneItem(Item innerItem)
            : base(innerItem)
        {
            _BaseStopProcessingActionItem = new BaseStopProcessingItem(innerItem);
        }

        public static implicit operator NoneItem(Item innerItem)
        {
            return innerItem != null ? new NoneItem(innerItem) : null;
        }

        public static implicit operator Item(NoneItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code
    }
}
