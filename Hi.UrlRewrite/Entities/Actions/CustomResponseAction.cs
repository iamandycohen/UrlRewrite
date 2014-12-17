namespace Hi.UrlRewrite.Entities.Actions
{
    public class CustomResponseAction : IBaseAction
    {
        public int StatusCode { get; set; }
        public int? SubStatusCode { get; set; }
        public string Reason { get; set; }
        public string ErrorDescription { get; set; }

        public string Name { get; set; }
    }
}
