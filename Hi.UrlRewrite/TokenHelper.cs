
namespace Hi.UrlRewrite
{
    public static class TokenHelper
    {
        public static string TokenString(Tokens token)
        {
            return string.Format("{{{0}}}", token);
        }

        public static string Formatted(this Tokens token)
        {
            return TokenString(token);
        }
    }
}