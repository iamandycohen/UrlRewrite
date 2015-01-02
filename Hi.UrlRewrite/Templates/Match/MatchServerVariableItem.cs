using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Hi.UrlRewrite.Templates.Match
{
    public partial class MatchServerVariableItem : CustomItem
    {

        public static readonly string TemplateId = "{31CBA307-BE81-4E55-96E4-62F70B2CBF47}";

        #region Boilerplate CustomItem Code

        public MatchServerVariableItem(Item innerItem)
            : base(innerItem)
        {
        }

        public static implicit operator MatchServerVariableItem(Item innerItem)
        {
            return innerItem != null ? new MatchServerVariableItem(innerItem) : null;
        }

        public static implicit operator Item(MatchServerVariableItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods

        public TextField ServerVariableName
        {
            get
            {
                return new TextField(InnerItem.Fields["Server Variable Name"]);
            }
        }

        #endregion //Field Instance Methods
    }
}