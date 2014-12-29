using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Hi.UrlRewrite.Processing.Results
{
    public class ConditionMatch
    {
        public Match Match { get; set; }
        public string ConditionInput { get; set; }
    }
}