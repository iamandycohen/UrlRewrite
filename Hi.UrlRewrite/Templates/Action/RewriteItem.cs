using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.UrlRewrite.Templates.Action.Base;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Hi.UrlRewrite.Templates.Action
{
    public partial class RewriteItem : CustomItem
    {

        public static readonly string TemplateId = "{7ACB0905-11D7-47BD-94B3-A903D264135F}";

        #region Inherited Base Templates

        private readonly BaseRewriteItem _BaseRewriteItem;
        public BaseRewriteItem BaseRewriteItem { get { return _BaseRewriteItem; } }

        #endregion

        #region Boilerplate CustomItem Code

        public RewriteItem(Item innerItem)
            : base(innerItem)
        {
            _BaseRewriteItem = new BaseRewriteItem(innerItem);
        }

        public static implicit operator RewriteItem(Item innerItem)
        {
            return innerItem != null ? new RewriteItem(innerItem) : null;
        }

        public static implicit operator Item(RewriteItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code

        public TextField LogRewrittenUrl
        {
            get
            {
                return new TextField(InnerItem.Fields["Log rewritten url"]);
            }
        }
    }
}
