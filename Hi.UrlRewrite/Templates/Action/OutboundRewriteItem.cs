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
    public class OutboundRewriteItem : CustomItem
    {
        public static readonly string TemplateId = "{A5E7B43A-4387-4109-950E-6181452F4976}";

        #region Inherited Base Templates

        private readonly BaseStopProcessingActionItem _BaseStopProcessingActionItem;
        public BaseStopProcessingActionItem BaseStopProcessingActionItem { get { return _BaseStopProcessingActionItem; } }

        #endregion

        #region Boilerplate CustomItem Code

        public OutboundRewriteItem(Item innerItem)
            : base(innerItem)
        {
            _BaseStopProcessingActionItem = new BaseStopProcessingActionItem(innerItem);
        }

        public static implicit operator OutboundRewriteItem(Item innerItem)
        {
            return innerItem != null ? new OutboundRewriteItem(innerItem) : null;
        }

        public static implicit operator Item(OutboundRewriteItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code

        #region Field Instance Methods

        public TextField Value
        {
            get
            {
                return new TextField(InnerItem.Fields["Value"]);
            }
        }

        #endregion

    }
}
