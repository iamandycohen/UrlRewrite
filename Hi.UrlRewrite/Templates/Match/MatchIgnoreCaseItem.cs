using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Hi.UrlRewrite.Templates.Match
{
    public partial class MatchIgnoreCaseItem : CustomItem
    {

        public static readonly string TemplateId = "{E4AB5966-0E72-431B-ABB3-8CB9274CC074}";


        #region Boilerplate CustomItem Code

        public MatchIgnoreCaseItem(Item innerItem)
            : base(innerItem)
        {

        }

        public static implicit operator MatchIgnoreCaseItem(Item innerItem)
        {
            return innerItem != null ? new MatchIgnoreCaseItem(innerItem) : null;
        }

        public static implicit operator Item(MatchIgnoreCaseItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods

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