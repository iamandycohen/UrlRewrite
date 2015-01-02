using Hi.UrlRewrite.Entities.Actions.Base;
using System;

namespace Hi.UrlRewrite.Entities.Actions
{
    [Serializable]
    public class AbortRequest : IBaseAction
    {
        public string Name { get; set; }
    }
}
