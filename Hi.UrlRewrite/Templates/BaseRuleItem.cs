using Hi.UrlRewrite.Templates.Conditions;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Hi.UrlRewrite.Templates
{
    public class BaseRuleItem : CustomItem
    {
        public static readonly string TemplateId = "{995478B6-29FB-4CF8-B5FB-5C1C5B21BF5A}";

        #region Inherited Base Templates

        private readonly BaseUrlRewriteItem _BaseUrlRewriteItem;
        public BaseUrlRewriteItem BaseUrlRewrite { get { return _BaseUrlRewriteItem; } }

        private readonly ConditionLogicalGroupingItem _ConditionLogicalGroupingItem;
        public ConditionLogicalGroupingItem ConditionLogicalGroupingItem { get { return _ConditionLogicalGroupingItem; } }

        private readonly BaseMatchItem _BaseMatchItem;
        public BaseMatchItem BaseMatchItem { get { return _BaseMatchItem; } }


        #endregion

        public BaseRuleItem(Item innerItem)
            : base(innerItem)
        {
            _BaseUrlRewriteItem = new BaseUrlRewriteItem(innerItem);
            _ConditionLogicalGroupingItem = new ConditionLogicalGroupingItem(innerItem);
            _BaseMatchItem = new BaseMatchItem(innerItem);
        }

        public static implicit operator BaseRuleItem(Item innerItem)
        {
            return innerItem != null ? new BaseRuleItem(innerItem) : null;
        }

        public static implicit operator Item(BaseRuleItem customItem)
        {
	        return customItem != null ? customItem.InnerItem : null;
        }

        public CheckboxField Enabled
        {
            get
            {
                return new CheckboxField(InnerItem.Fields["Enabled"]);
            }
        }
            
    }
}
