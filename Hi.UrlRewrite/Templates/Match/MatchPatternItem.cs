using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Hi.UrlRewrite.Templates.Match
{
    public partial class MatchPatternItem : CustomItem
    {

        public static readonly string TemplateId = "{E4AB5966-0E72-431B-ABB3-8CB9274CC074}";


        #region Boilerplate CustomItem Code

        public MatchPatternItem(Item innerItem)
            : base(innerItem)
        {

        }

        public static implicit operator MatchPatternItem(Item innerItem)
        {
            return innerItem != null ? new MatchPatternItem(innerItem) : null;
        }

        public static implicit operator Item(MatchPatternItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods

        public TextField Pattern
        {
            get
            {
                return new TextField(InnerItem.Fields["Pattern"]);
            }
        }

        #endregion //Field Instance Methods
    }
}