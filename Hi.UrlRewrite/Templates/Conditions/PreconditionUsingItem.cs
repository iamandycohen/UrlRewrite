using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Hi.UrlRewrite.Templates.Conditions
{
    public partial class PreconditionUsingItem : CustomItem
    {

        public static readonly string TemplateId = "{20C923EF-DAD7-433E-B51C-A5B634C3A27B}";


        #region Boilerplate CustomItem Code

        public PreconditionUsingItem(Item innerItem)
            : base(innerItem)
        {

        }

        public static implicit operator PreconditionUsingItem(Item innerItem)
        {
            return innerItem != null ? new PreconditionUsingItem(innerItem) : null;
        }

        public static implicit operator Item(PreconditionUsingItem customItem)
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