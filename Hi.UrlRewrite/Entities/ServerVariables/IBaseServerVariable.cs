using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.UrlRewrite.Entities.ServerVariables
{
    public interface IBaseServerVariable
    {
        string Name { get; set; }
        string VariableName { get; set; }
        string Value { get; set; }
        bool ReplaceExistingValue { get; set; }
    }
}
