using Hi.UrlRewrite.Templates.Conditions;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Hi.UrlRewrite.Templates
{
    public class BaseHitCountItem : CustomItem
    {
        public static readonly string TemplateId = "{A012FA42-BB6D-414F-B78B-C1576B3E1B8B}";

        #region Inherited Base Templates

        #endregion

        public BaseHitCountItem(Item innerItem)
            : base(innerItem)
        {
        }

        public static implicit operator BaseHitCountItem(Item innerItem)
        {
            return innerItem != null ? new BaseHitCountItem(innerItem) : null;
        }

        public static implicit operator Item(BaseHitCountItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        public TextField HitCount
        {
            get
            {
                return new TextField(InnerItem.Fields["Hit Count"]);
            }
        }

    }
}
