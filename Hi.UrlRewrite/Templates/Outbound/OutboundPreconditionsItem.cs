using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Hi.UrlRewrite.Templates.Outbound
{
    public class OutboundPreconditionsItem : CustomItem
    {
        public static readonly string TemplateId = "{542B92EE-1A9C-48FC-A81B-B034CB6AE368}";

        #region Inherited Base Templates

        #endregion

        public OutboundPreconditionsItem(Item innerItem)
            : base(innerItem)
        {

        }

        public static implicit operator OutboundPreconditionsItem(Item innerItem)
        {
            return innerItem != null ? new OutboundPreconditionsItem(innerItem) : null;
        }

        public static implicit operator Item(OutboundPreconditionsItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        public LookupField Precondition
        {
            get
            {
                return new LookupField(InnerItem.Fields["Precondition"]);
            }
        }
            
    }
}
