using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Hi.UrlRewrite.Templates.Match
{
    public partial class MatchMatchTypeItem : CustomItem
    {

        public static readonly string TemplateId = "{07597712-CC43-4AE3-BD01-01B9EB284261}";


        #region Boilerplate CustomItem Code

        public MatchMatchTypeItem(Item innerItem)
            : base(innerItem)
        {

        }

        public static implicit operator MatchMatchTypeItem(Item innerItem)
        {
            return innerItem != null ? new MatchMatchTypeItem(innerItem) : null;
        }

        public static implicit operator Item(MatchMatchTypeItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods

        public LookupField MatchType
        {
            get
            {
                return new LookupField(InnerItem.Fields["Match Type"]);
            }
        }

        #endregion //Field Instance Methods
    }
}