using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.UrlRewrite.Entities
{
    public class RedirectAction : BaseRedirectAction
    {
        public RedirectType? RedirectType { get; set; }
    }
}
