using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.UrlRewrite.Entities.Match
{
    public interface IMatchServerVariable : IBaseMatchScope
    {
        string ServerVariableName { get; set; }
    }
}
