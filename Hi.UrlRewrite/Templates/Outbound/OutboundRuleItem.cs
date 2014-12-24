using Hi.UrlRewrite.Templates.Conditions;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Hi.UrlRewrite.Templates.Outbound
{
    public class OutboundRuleItem : CustomItem
    {
        public static readonly string TemplateId = "{DC4D631D-E17D-4F18-A19F-3554CD4EB756}";

        #region Inherited Base Templates

        private readonly BaseRuleItem _BaseRuleItem;
        public BaseRuleItem BaseRuleItem { get { return _BaseRuleItem; } }

        private readonly OutboundPreconditionItem _OutboundPreconditionItem;
        public OutboundPreconditionItem OutboundPreconditionItem { get { return _OutboundPreconditionItem; } }

        private readonly OutboundMatchItem _OutboundMatchItem;
        public OutboundMatchItem OutboundMatchItem { get { return _OutboundMatchItem; } }

        #endregion

        public OutboundRuleItem(Item innerItem)
            : base(innerItem)
        {
            _BaseRuleItem = new BaseRuleItem(innerItem);
            _OutboundPreconditionItem = new OutboundPreconditionItem(innerItem);
            _OutboundMatchItem = new OutboundMatchItem(innerItem);
        }

        public static implicit operator OutboundRuleItem(Item innerItem)
        {
            return innerItem != null ? new OutboundRuleItem(innerItem) : null;
        }

        public static implicit operator Item(OutboundRuleItem customItem)
        {
	        return customItem != null ? customItem.InnerItem : null;
        }

        public LookupField Action
        {
            get
            {
                return new LookupField(InnerItem.Fields["Action"]);
            }
        }

            
    }
}
