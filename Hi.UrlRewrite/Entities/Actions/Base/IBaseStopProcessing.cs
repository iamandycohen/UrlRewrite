namespace Hi.UrlRewrite.Entities.Actions.Base
{
    public interface IBaseStopProcessing : IBaseAction
    {
        bool StopProcessingOfSubsequentRules { get; set; }
    }
}
