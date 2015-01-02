using System;

namespace Hi.UrlRewrite.Entities.Actions.Base
{
    public interface IBaseRewriteUrl
    {
        string RewriteUrl { get; set; }
        Guid? RewriteItemId { get; set; }
        string RewriteItemAnchor { get; set; }
    }
}
