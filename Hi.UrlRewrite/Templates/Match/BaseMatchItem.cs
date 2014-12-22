using System;
using Hi.UrlRewrite.Templates.Match;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;

namespace Hi.UrlRewrite.Templates.Conditions
{
    public partial class BaseMatchItem : CustomItem
    {

        public static readonly string TemplateId = "{57516483-A64C-4036-895B-B55D9267A8E6}";

        #region Inherited Base Templates

        private readonly MatchIgnoreCaseItem _MatchIgnoreCaseItem;
        public MatchIgnoreCaseItem MatchIgnoreCaseItem { get { return _MatchIgnoreCaseItem; } }

        private readonly MatchMatchTypeItem _MatchMatchTypeItem;
        public MatchMatchTypeItem MatchMatchTypeItem { get { return _MatchMatchTypeItem; } }

        private readonly MatchPatternItem _MatchPatternItem;
        public MatchPatternItem MatchPatternItem { get { return _MatchPatternItem; } }

        private readonly MatchUsingItem _MatchUsingItem;
        public MatchUsingItem MatchUsingItem { get { return _MatchUsingItem; } }

        #endregion

        #region Boilerplate CustomItem Code

        public BaseMatchItem(Item innerItem)
            : base(innerItem)
        {
            _MatchIgnoreCaseItem = new MatchIgnoreCaseItem(innerItem);
            _MatchMatchTypeItem = new MatchMatchTypeItem(innerItem);
            _MatchPatternItem = new MatchPatternItem(innerItem);
            _MatchUsingItem = new MatchUsingItem(innerItem);
        }

        public static implicit operator BaseMatchItem(Item innerItem)
        {
            return innerItem != null ? new BaseMatchItem(innerItem) : null;
        }

        public static implicit operator Item(BaseMatchItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods

        #endregion //Field Instance Methods
    }
}