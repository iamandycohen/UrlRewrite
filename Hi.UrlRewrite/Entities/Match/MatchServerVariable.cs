using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hi.UrlRewrite.Entities.Match
{
    [Serializable]
    public class MatchServerVariable : IMatchServerVariable
    {
        public string ServerVariableName { get; set; }
    }
}