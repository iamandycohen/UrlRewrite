using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Hi.UrlRewrite.Templates.Match
{
    public partial class MatchUsingItem : CustomItem
    {

        public static readonly string TemplateId = "{BD62F334-3EAB-483D-A88B-D1D36EE51C9E}";


        #region Boilerplate CustomItem Code

        public MatchUsingItem(Item innerItem)
            : base(innerItem)
        {

        }

        public static implicit operator MatchUsingItem(Item innerItem)
        {
            return innerItem != null ? new MatchUsingItem(innerItem) : null;
        }

        public static implicit operator Item(MatchUsingItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods

        public LookupField Using
        {
            get
            {
                return new LookupField(InnerItem.Fields["Using"]);
            }
        }


        #endregion //Field Instance Methods
    }
}