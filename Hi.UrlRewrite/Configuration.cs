using Sitecore.Configuration;

namespace Hi.UrlRewrite
{
    public class Configuration
    {
        public static string[] IgnoreUrlPrefixes
        {
            get
            {
                return Settings.GetSetting("Hi.UrlRewrite.IgnoreUrlPrefixes", "/sitecore").Split('|');
            }
        }

        public static string RewriteFolderSearchRoot
        {
            get
            {
                return Settings.GetSetting("Hi.UrlRewrite.RewriteFolderSearchRoot", "/sitecore");
            }
        }

        public static string CacheSize
        {
            get
            {
                return Settings.GetSetting("Hi.UrlRewrite.CacheSize", "10MB");
            }
        }
    }
}
