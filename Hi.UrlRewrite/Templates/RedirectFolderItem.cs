using System;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;

namespace Hi.UrlRewrite.Templates
{
    public partial class RedirectFolderItem : CustomItem
    {

        public static readonly string TemplateId = "{CBE995D0-FCE0-4061-B807-B4BBC89962A7}";

        #region Inherited Base Templates

        private readonly FolderItem _Folder;
        public FolderItem Folder { get { return _Folder; } }

        private readonly BaseUrlRewriteItem _BaseUrlRewriteItem;
        public BaseUrlRewriteItem BaseUrlRewrite { get { return _BaseUrlRewriteItem; } }

        #endregion

        #region Boilerplate CustomItem Code

        public RedirectFolderItem(Item innerItem)
            : base(innerItem)
        {
            _Folder = new FolderItem(innerItem);
            _BaseUrlRewriteItem = new BaseUrlRewriteItem(innerItem);
        
        }

        public static implicit operator RedirectFolderItem(Item innerItem)
        {
            return innerItem != null ? new RedirectFolderItem(innerItem) : null;
        }

        public static implicit operator Item(RedirectFolderItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods

        public TextField SitePattern
        {
            get
            {
                return new TextField(InnerItem.Fields["Site Pattern"]);
            }
        }

        #endregion //Field Instance Methods
    }
}

