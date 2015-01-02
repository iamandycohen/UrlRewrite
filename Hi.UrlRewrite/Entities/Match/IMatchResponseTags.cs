using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.UrlRewrite.Entities.Match
{
    public interface IMatchResponseTags : IBaseMatchScope
    {
        IEnumerable<MatchTag> MatchTheContentWithin { get; set; }
    }
}
