using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.UrlRewrite.Entities
{
    public class Condition
    {
        public string Name { get; set; }
        public ConditionInputType? ConditionInput { get; set; }
        public CheckIfInputStringType? CheckIfInputString { get; set; }
        public string Pattern { get; set; }
        public bool IgnoreCase { get; set; }
    }
}
