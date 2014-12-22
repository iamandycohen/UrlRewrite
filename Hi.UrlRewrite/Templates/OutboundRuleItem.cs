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
    public class OutboundRuleItem : CustomItem
    {
        public static readonly string TemplateId = "{DC4D631D-E17D-4F18-A19F-3554CD4EB756}";

        #region Inherited Base Templates

        private readonly BaseRuleItem _BaseRuleItem;
        public BaseRuleItem BaseRuleItem { get { return _BaseRuleItem; } }

        #endregion

        public OutboundRuleItem(Item innerItem)
            : base(innerItem)
        {
            _BaseRuleItem = new BaseRuleItem(innerItem);
        }

        public static implicit operator OutboundRuleItem(Item innerItem)
        {
            return innerItem != null ? new OutboundRuleItem(innerItem) : null;
        }

        public static implicit operator Item(OutboundRuleItem customItem)
        {
	        return customItem != null ? customItem.InnerItem : null;
        }

        //public LookupField Action
        //{
        //    get
        //    {
        //        return new LookupField(InnerItem.Fields["Action"]);
        //    }
        //}
            
    }
}
