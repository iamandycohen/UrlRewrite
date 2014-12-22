using Sitecore.Data.Items;

namespace Hi.UrlRewrite.Templates.Match.Types
{
    public partial class UsingTypeItem : CustomItem
    {

        public static readonly string TemplateId = "{7CBECFAE-2E9E-4E79-A6ED-02536EAFA383}";


        #region Boilerplate CustomItem Code

        public UsingTypeItem(Item innerItem)
            : base(innerItem)
        {

        }

        public static implicit operator UsingTypeItem(Item innerItem)
        {
            return innerItem != null ? new UsingTypeItem(innerItem) : null;
        }

        public static implicit operator Item(UsingTypeItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods


        #endregion //Field Instance Methods
    }
}