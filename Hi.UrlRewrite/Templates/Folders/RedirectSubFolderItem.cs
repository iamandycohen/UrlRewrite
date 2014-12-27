using Hi.UrlRewrite.Templates.Settings;
using Sitecore.Data.Items;

namespace Hi.UrlRewrite.Templates.Folders
{
    public partial class RedirectSubFolderItem : CustomItem
    {

        public static readonly string TemplateId = "{9461E537-8E89-4B91-896A-1F2C3AF4A3D5}";

        #region Inherited Base Templates

        private readonly FolderItem _FolderItem;
        public FolderItem FolderItem { get { return _FolderItem; } }

        private readonly BaseUrlRewriteItem _BaseUrlRewriteItem;
        public BaseUrlRewriteItem BaseUrlRewriteItem { get { return _BaseUrlRewriteItem; } }

        #endregion

        #region Boilerplate CustomItem Code

        public RedirectSubFolderItem(Item innerItem)
            : base(innerItem)
        {
            _FolderItem = new FolderItem(innerItem);
            _BaseUrlRewriteItem = new BaseUrlRewriteItem(innerItem);
        
        }

        public static implicit operator RedirectSubFolderItem(Item innerItem)
        {
            return innerItem != null ? new RedirectSubFolderItem(innerItem) : null;
        }

        public static implicit operator Item(RedirectSubFolderItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


    }
}

