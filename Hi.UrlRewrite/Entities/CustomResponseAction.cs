using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.UrlRewrite.Entities
{
    public class CustomResponseAction : BaseAction
    {
        public int StatusCode { get; set; }
        public int SubStatusCode { get; set; }
        public string Reason { get; set; }
        public string ErrorDescription { get; set; }
    }
}
