using System;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;

namespace Hi.UrlRewrite.Templates.Conditions
{
    public partial class ConditionItem : CustomItem
    {

        public static readonly string TemplateId = "{2083F66B-0A94-4F9C-9833-EF53FAD05D70}";

        #region Inherited Base Templates

        private readonly BaseUrlRewriteItem _BaseUrlRewriteItem;
        public BaseUrlRewriteItem BaseUrlRewrite { get { return _BaseUrlRewriteItem; } }

        #endregion

        #region Boilerplate CustomItem Code

        public ConditionItem(Item innerItem)
            : base(innerItem)
        {
            _BaseUrlRewriteItem = new BaseUrlRewriteItem(innerItem);
        }

        public static implicit operator ConditionItem(Item innerItem)
        {
            return innerItem != null ? new ConditionItem(innerItem) : null;
        }

        public static implicit operator Item(ConditionItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods


        public LookupField ConditionInput
        {
            get
            {
                return new LookupField(InnerItem.Fields["Condition Input"]);
            }
        }


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