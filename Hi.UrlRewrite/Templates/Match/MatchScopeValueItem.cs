using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Hi.UrlRewrite.Templates.Match
{
    public partial class MatchScopeValueItem : CustomItem
    {

        public static readonly string TemplateId = "{7600DAF1-86EB-4E46-813A-A2971B7B5722}";


        #region Boilerplate CustomItem Code

        public MatchScopeValueItem(Item innerItem)
            : base(innerItem)
        {

        }

        public static implicit operator MatchScopeValueItem(Item innerItem)
        {
            return innerItem != null ? new MatchScopeValueItem(innerItem) : null;
        }

        public static implicit operator Item(MatchScopeValueItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods

        public TextField ScopeValue
        {
            get
            {
                return new TextField(InnerItem.Fields["Scope value"]);
            }
        }

        #endregion //Field Instance Methods
    }
}