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

        private readonly BaseUrlRewriteItem _BaseUrlRewriteItem;
        public BaseUrlRewriteItem BaseUrlRewrite { get { return _BaseUrlRewriteItem; } }

        #endregion

        public InboundRuleItem(Item innerItem)
            : base(innerItem)
        {
            _BaseUrlRewriteItem = new BaseUrlRewriteItem(innerItem);
        
        }

        public static implicit operator InboundRuleItem(Item innerItem)
        {
            return innerItem != null ? new InboundRuleItem(innerItem) : null;
        }

        public static implicit operator Item(InboundRuleItem customItem)
        {
	        return customItem != null ? customItem.InnerItem : null;
        }

        public LookupField RequestedUrl
        {
            get
            {
                return new LookupField(InnerItem.Fields["Requested Url"]);
            }
        }

        public LookupField Using
        {
            get
            {
                return new LookupField(InnerItem.Fields["Using"]);
            }
        }

        public TextField Pattern
        {
            get
            {
                return new TextField(InnerItem.Fields["Pattern"]);
            }
        }

        public CheckboxField IgnoreCase
        {
            get
            {
                return new CheckboxField(InnerItem.Fields["Ignore case"]);
            }
        }

        public LookupField Action
        {
            get
            {
                return new LookupField(InnerItem.Fields["Action"]);
            }
        }

        public LookupField LogicalGrouping
        {
            get
            {
                return new LookupField(InnerItem.Fields["Logical grouping"]);
            }
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
