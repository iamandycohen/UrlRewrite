using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.UrlRewrite.Entities.Actions
{
    public interface IBaseStatusCode
    {
        RedirectActionStatusCode? StatusCode { get; set; }
    }
}
