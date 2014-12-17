namespace Hi.UrlRewrite.Entities.Actions
{
    public interface IBaseStopProcessingAction : IBaseAction
    {
        bool StopProcessingOfSubsequentRules { get; set; }
    }
}
