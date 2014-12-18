using System;

namespace Hi.UrlRewrite.Entities.Actions
{
    [Serializable]
    public class AbortRequestAction : IBaseAction
    {
        public string Name { get; set; }
    }
}
