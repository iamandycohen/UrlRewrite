using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.UrlRewrite.Entities.ServerVariables
{
    interface IResponseHeaderList
    {
        IEnumerable<ResponseHeader> ResponseHeaders { get; set; }
    }
}
