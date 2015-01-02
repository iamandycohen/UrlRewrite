using System.Web;

namespace Hi.UrlRewrite.Entities.Actions.Base
{
    public interface IBaseCache
    {
        HttpCacheability? HttpCacheability { get; set; }
    }
}
