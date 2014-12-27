using System;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;

namespace Hi.UrlRewrite.Templates.Conditions
{
    public partial class BaseConditionItem : CustomItem
    {

        public static readonly string TemplateId = "{1CDF3BDD-0A44-42CD-9D40-FBA83B981F48}";

        #region Inherited Base Templates

        private readonly BaseUrlRewriteItem _BaseUrlRewriteItem;
        public BaseUrlRewriteItem BaseUrlRewriteItem { get { return _BaseUrlRewriteItem; } }

        #endregion

        #region Boilerplate CustomItem Code

        public BaseConditionItem(Item innerItem)
            : base(innerItem)
        {
            _BaseUrlRewriteItem = new BaseUrlRewriteItem(innerItem);
        }

        public static implicit operator BaseConditionItem(Item innerItem)
        {
            return innerItem != null ? new BaseConditionItem(innerItem) : null;
        }

        public static implicit operator Item(BaseConditionItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods

        public LookupField CheckIfInputString
        {
            get
            {
                return new LookupField(InnerItem.Fields["Check if input string"]);
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

        #endregion //Field Instance Methods
    }
}