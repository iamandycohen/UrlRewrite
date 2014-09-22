using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Hi.UrlRewrite.Entities
{
    public interface IBaseCache
    {
        HttpCacheability? HttpCacheability { get; set; }
    }
}
