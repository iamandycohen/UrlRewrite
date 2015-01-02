using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Hi.UrlRewrite.Templates.Match
{
    public partial class MatchResponseTagsItem : CustomItem
    {

        public static readonly string TemplateId = "{E07F2DBA-75E2-4621-91B4-A1A756289D20}";

        #region Boilerplate CustomItem Code

        public MatchResponseTagsItem(Item innerItem)
            : base(innerItem)
        {
        }

        public static implicit operator MatchResponseTagsItem(Item innerItem)
        {
            return innerItem != null ? new MatchResponseTagsItem(innerItem) : null;
        }

        public static implicit operator Item(MatchResponseTagsItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods

        public MultilistField MatchTheContentWithin
        {
            get
            {
                return new MultilistField(InnerItem.Fields["Match the content within"]);
            }
        }

        #endregion //Field Instance Methods
    }
}