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

        private readonly BaseConditionItem _BaseConditionItem;
        public BaseConditionItem BaseConditionItem { get { return _BaseConditionItem; } }

        #endregion

        #region Boilerplate CustomItem Code

        public ConditionItem(Item innerItem)
            : base(innerItem)
        {
            _BaseConditionItem = new BaseConditionItem(innerItem);
        }

        public static implicit operator ConditionItem(Item innerItem)
        {
            return innerItem != null ? new ConditionItem(innerItem) : null;
        }

        public static implicit operator Item(ConditionItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        //public static implicit operator ConditionItem(BaseConditionItem baseConditionItem)
        //{
        //    return baseConditionItem != null ? new ConditionItem(baseConditionItem.InnerItem) : null;
        //}

        //public static implicit operator BaseConditionItem(ConditionItem customItem)
        //{
        //    return customItem != null ? customItem.BaseConditionItem : null;
        //}

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods


        public LookupField ConditionInput
        {
            get
            {
                return new LookupField(InnerItem.Fields["Condition Input Type"]);
            }
        }

        #endregion //Field Instance Methods
    }
}