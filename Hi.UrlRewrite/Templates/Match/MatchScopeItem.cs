using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Hi.UrlRewrite.Templates.Match
{
    public partial class MatchScopeItem : CustomItem
    {

        public static readonly string TemplateId = "{7B5B7C00-4708-485B-809E-1064B689B2DE}";


        #region Boilerplate CustomItem Code

        public MatchScopeItem(Item innerItem)
            : base(innerItem)
        {

        }

        public static implicit operator MatchScopeItem(Item innerItem)
        {
            return innerItem != null ? new MatchScopeItem(innerItem) : null;
        }

        public static implicit operator Item(MatchScopeItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods

        public LookupField MatchingScope
        {
            get
            {
                return new LookupField(InnerItem.Fields["Matching scope"]);
            }
        }

        #endregion //Field Instance Methods
    }
}