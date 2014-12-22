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

        private readonly BaseUrlRewriteItem _BaseUrlRewriteItem;
        public BaseUrlRewriteItem BaseUrlRewrite { get { return _BaseUrlRewriteItem; } }

        private readonly RuleConditionPropertiesItem _RuleConditionPropertiesItem;
        public RuleConditionPropertiesItem RuleConditionPropertiesItem { get { return _RuleConditionPropertiesItem; } }

        private readonly BaseMatchItem _BaseMatchItem;
        public BaseMatchItem BaseMatchItem { get { return _BaseMatchItem; } }


        #endregion

        public OutboundRuleItem(Item innerItem)
            : base(innerItem)
        {
            _BaseUrlRewriteItem = new BaseUrlRewriteItem(innerItem);
            _RuleConditionPropertiesItem = new RuleConditionPropertiesItem(innerItem);
            _BaseMatchItem = new BaseMatchItem(innerItem);
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

        //public CheckboxField Enabled
        //{
        //    get
        //    {
        //        return new CheckboxField(InnerItem.Fields["Enabled"]);
        //    }
        //}
            
    }
}
