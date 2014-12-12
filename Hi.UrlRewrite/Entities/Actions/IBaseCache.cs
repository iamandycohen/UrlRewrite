using System.Web;

namespace Hi.UrlRewrite.Entities.Actions
{
    public interface IBaseCache
    {
        HttpCacheability? HttpCacheability { get; set; }
    }
}
