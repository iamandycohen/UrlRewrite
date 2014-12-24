using Sitecore.Data;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SLog = Sitecore.Diagnostics.Log;

namespace Hi.UrlRewrite
{
    public class Log
    {
        static Log()
        {
            LogHelper.Inititalize();
        }

        private const string LogName = "UrlRewrite";
        private const string LogNameDbFormat = LogName + "[{0}]";
        private const string Divider = "::";

        public static void Info(object owner, string format, params object[] args)
        {
            SLog.Info(string.Format(format, args), owner);
        }

        public static void Info(object owner, Database database, string format, params object[] args)
        {
            SLog.Info(Format(database, format, args), owner);
        }

        public static void Debug(object owner, string format, params object[] args)
        {
            SLog.Debug(Format(format, args), owner);
        }

        public static void Debug(object owner, Database database, string format, params object[] args)
        {
            SLog.Debug(Format(database, format, args), owner);
        }

        public static void Warn(object owner, string format, params object[] args)
        {
            SLog.Warn(Format(format, args), owner);
        }

        public static void Warn(object owner, Database database, string format, params object[] args)
        {
            SLog.Warn(Format(database, format, args), owner);
        }

        public static void Error(object owner, Exception exception, string format, params object[] args)
        {
            SLog.Error(Format(format, args), exception, owner);
        }

        public static void Error(object owner, Exception exception, Database database, string format, params object[] args)
        {
            SLog.Error(Format(database, format, args), exception, owner);
        }

        public static void Error(object owner, string format, params object[] args)
        {
            SLog.Error(Format(format, args), owner);
        }

        public static void Error(object owner, Database database, string format, params object[] args)
        {
            SLog.Error(Format(database, format, args), owner);
        }

        private static string Format(string format, object[] args)
        {
            var db = Sitecore.Context.Database;
            if (db != null)
            {
                return Format(db, format, args);
            }

            return LogName + Divider + string.Format(format, args);
        }

        private static string Format(Database database, string format, object[] args)
        {
            if (database == null)
            {
                return Format(format, args);
            }

            return string.Format(LogNameDbFormat, database.Name) + Divider + string.Format(format, args);
        }

    }
}