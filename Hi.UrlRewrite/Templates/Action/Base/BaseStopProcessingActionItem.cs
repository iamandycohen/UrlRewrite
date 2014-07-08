using System;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;

namespace Hi.UrlRewrite.Templates.Action.Base
{
    public partial class BaseStopProcessingActionItem : CustomItem
    {

        public static readonly string TemplateId = "{05BB43CA-F36D-46CF-BBAB-D46C8E3FEF16}";

        #region Inherited Base Templates

        private readonly BaseActionItem _BaseAction;
        public BaseActionItem BaseAction { get { return _BaseAction; } }

        #endregion

        #region Boilerplate CustomItem Code

        public BaseStopProcessingActionItem(Item innerItem)
            : base(innerItem)
        {
            _BaseAction = new BaseActionItem(innerItem);

        }

        public static implicit operator BaseStopProcessingActionItem(Item innerItem)
        {
            return innerItem != null ? new BaseStopProcessingActionItem(innerItem) : null;
        }

        public static implicit operator Item(BaseStopProcessingActionItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods


        public CheckboxField StopProcessingOfSubsequentRules
        {
            get
            {
                return new CheckboxField(InnerItem.Fields["Stop processing of subsequent rules"]);
            }
        }


        #endregion //Field Instance Methods
    }
}