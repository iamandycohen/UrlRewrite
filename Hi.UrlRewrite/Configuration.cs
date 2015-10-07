using Sitecore.Configuration;
using System;

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
                return Settings.GetSetting("Hi.UrlRewrite.RewriteFolderSearchRoot", "/sitecore/content");
            }
        }

        public static string CacheSize
        {
            get
            {
                return Settings.GetSetting("Hi.UrlRewrite.CacheSize", "10MB");
            }
        }

        public static bool LogFileEnabled
        {
            get
            {
                return Settings.GetBoolSetting("Hi.UrlRewrite.LogFileEnabled", false);
            }
        }

        public static string LogFileName
        {
            get
            {
                return Settings.GetSetting("Hi.UrlRewrite.LogFileName", @"$(dataFolder)/logs/UrlRewrite.log.{date}.txt");
            }
        }

        public static string LogFileLevel
        {
            get
            {
                return Settings.GetSetting("Hi.UrlRewrite.LogFileLevel", "INFO");
            }
        }

        public static string ItemWebApiHost
        {
            get
            {
                return Settings.GetSetting("Hi.UrlRewrite.ItemWebApiHost", @"http://andysplayground");
            }
        }

        public static string ItemWebApiUser
        {
            get
            {
                return Settings.GetSetting("Hi.UrlRewrite.ItemWebApiUser", @"sitecore\admin");
            }
        }

        public static string ItemWebApiPassword
        {
            get
            {
                return Settings.GetSetting("Hi.UrlRewrite.ItemWebApiPassword", "b");
            }
        }

    }
}
