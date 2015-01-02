using Hi.UrlRewrite.Entities.Actions.Base;
using System;

namespace Hi.UrlRewrite.Entities.Actions
{
    [Serializable]
    public class CustomResponse : IBaseAction
    {
        public int StatusCode { get; set; }
        public int? SubStatusCode { get; set; }
        public string Reason { get; set; }
        public string ErrorDescription { get; set; }

        public string Name { get; set; }
    }
}
