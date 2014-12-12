namespace Hi.UrlRewrite.Entities.Actions
{
    public class BaseStopProcessingAction : BaseAction
    {
        public bool StopProcessingOfSubsequentRules { get; set; }
    }
}
