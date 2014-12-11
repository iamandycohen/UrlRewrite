using System;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;

namespace Hi.UrlRewrite.Templates
{
    public partial class RedirectSubFolderItem : CustomItem
    {

        public static readonly string TemplateId = "{9461E537-8E89-4B91-896A-1F2C3AF4A3D5}";

        #region Inherited Base Templates

        private readonly FolderItem _Folder;
        public FolderItem Folder { get { return _Folder; } }

        private readonly BaseUrlRewriteItem _BaseUrlRewriteItem;
        public BaseUrlRewriteItem BaseUrlRewrite { get { return _BaseUrlRewriteItem; } }

        #endregion

        #region Boilerplate CustomItem Code

        public RedirectSubFolderItem(Item innerItem)
            : base(innerItem)
        {
            _Folder = new FolderItem(innerItem);
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

