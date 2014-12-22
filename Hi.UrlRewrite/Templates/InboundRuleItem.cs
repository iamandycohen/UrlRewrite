using Hi.UrlRewrite.Templates.Conditions;
using Hi.UrlRewrite.Templates.Match;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.UrlRewrite.Templates
{
    public class InboundRuleItem : CustomItem
    {
        public static readonly string TemplateId = "{69DCE9A6-D8C1-463D-AF95-B7FEB326013F}";

        #region Inherited Base Templates

        private readonly BaseRuleItem _BaseRuleItem;
        public BaseRuleItem BaseRuleItem { get { return _BaseRuleItem; } }


        #endregion

        public InboundRuleItem(Item innerItem)
            : base(innerItem)
        {
            _BaseRuleItem = new BaseRuleItem(innerItem);
        }

        public static implicit operator InboundRuleItem(Item innerItem)
        {
            return innerItem != null ? new InboundRuleItem(innerItem) : null;
        }

        public static implicit operator Item(InboundRuleItem customItem)
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
