using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hi.UrlRewrite.Entities.Match
{
    [Serializable]
    public class MatchTag
    {
        public string Tag { get; set; }
        public string Attribute { get; set; }
    }
}