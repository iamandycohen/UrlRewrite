using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.UrlRewrite.Entities
{
    public enum CheckIfInputStringType
    {
        IsAFile,
        IsNotAFile,
        IsADirectory,
        IsNotADirctory,
        MatchesThePattern,
        DoesNotMatchThePattern
    }
}
