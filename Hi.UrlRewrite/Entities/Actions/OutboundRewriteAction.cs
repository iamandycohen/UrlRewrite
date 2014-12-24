using System;

namespace Hi.UrlRewrite.Entities.Actions
{
    [Serializable]
    public class OutboundRewriteAction : IBaseStopProcessingAction
    {
        public bool StopProcessingOfSubsequentRules { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}