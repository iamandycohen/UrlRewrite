using Hi.UrlRewrite.Entities.Actions.Base;
using System;

namespace Hi.UrlRewrite.Entities.Actions
{
    [Serializable]
    public class OutboundRewrite : IBaseStopProcessing
    {
        public bool StopProcessingOfSubsequentRules { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}