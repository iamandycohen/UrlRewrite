using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.UrlRewrite.Entities
{
    public class BaseStopProcessingAction : BaseAction
    {
        public bool StopProcessingOfSubsequentRules { get; set; }
    }
}
