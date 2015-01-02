using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Hi.UrlRewrite.Templates.Match
{
    public partial class MatchScopeTypeItem : CustomItem
    {

        public static readonly string TemplateId = "{7B5B7C00-4708-485B-809E-1064B689B2DE}";


        #region Boilerplate CustomItem Code

        public MatchScopeTypeItem(Item innerItem)
            : base(innerItem)
        {

        }

        public static implicit operator MatchScopeTypeItem(Item innerItem)
        {
            return innerItem != null ? new MatchScopeTypeItem(innerItem) : null;
        }

        public static implicit operator Item(MatchScopeTypeItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods

        public LookupField MatchScopeType
        {
            get
            {
                return new LookupField(InnerItem.Fields["Match Scope Type"]);
            }
        }

        #endregion //Field Instance Methods
    }
}