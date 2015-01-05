using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hi.UrlRewrite.Entities.ServerVariables
{
    [Serializable]
    public class ResponseHeader : IBaseServerVariable
    {
        public string Name { get; set; }
        public string VariableName { get; set; }
        public string Value { get; set; }
        public bool ReplaceExistingValue { get; set; }
    }
}