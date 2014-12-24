using Hi.UrlRewrite.Templates.Settings;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Hi.UrlRewrite.Templates.Folders
{
    public partial class PreconditionsFolderItem : CustomItem
    {

        public static readonly string TemplateId = "{9675D02E-D173-4760-8F13-3432B920D771}";

        #region Inherited Base Templates

        private readonly FolderItem _Folder;
        public FolderItem Folder { get { return _Folder; } }

        private readonly BaseUrlRewriteItem _BaseUrlRewriteItem;
        public BaseUrlRewriteItem BaseUrlRewrite { get { return _BaseUrlRewriteItem; } }

        #endregion

        #region Boilerplate CustomItem Code

        public PreconditionsFolderItem(Item innerItem)
            : base(innerItem)
        {
            _Folder = new FolderItem(innerItem);
            _BaseUrlRewriteItem = new BaseUrlRewriteItem(innerItem);
        
        }

        public static implicit operator PreconditionsFolderItem(Item innerItem)
        {
            return innerItem != null ? new PreconditionsFolderItem(innerItem) : null;
        }

        public static implicit operator Item(PreconditionsFolderItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods

        #endregion //Field Instance Methods
    }
}

