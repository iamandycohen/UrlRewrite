using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.UrlRewrite.Entities
{
    public enum RedirectType
    {
        Permanent = 301,
        Found = 302,
        SeeOther = 303,
        Temporary = 307
    }
}
