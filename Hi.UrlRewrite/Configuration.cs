using Sitecore.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
