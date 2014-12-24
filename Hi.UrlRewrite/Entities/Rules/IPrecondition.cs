using Hi.UrlRewrite.Entities.Conditions;

namespace Hi.UrlRewrite.Entities.Rules
{
    public interface IPrecondition
    {
        Precondition Precondition { get; set; }
    }
}