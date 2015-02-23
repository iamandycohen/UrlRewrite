using Sitecore.Rules;
using Sitecore.Rules.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using Hi.UrlRewrite.Module;

namespace Hi.UrlRewrite.Rules
{
    public class OutbounModuleIsInstalled<T> : WhenCondition<T> where T:RuleContext
    {
        protected override bool Execute(T ruleContext)
        {
            return Check();
        }

        private static bool? isInstalled;

        private static bool Check()
        {
            bool check = false;

            if (isInstalled.HasValue)
            {
                check = isInstalled.Value;
            }
            else if (HttpContext.Current != null)
            {
                var modules = HttpContext.Current.ApplicationInstance.Modules;

                isInstalled = check = modules.Cast<string>().Any(key => modules[key] is OutboundModule);
            }

            return check;
        }
    }
}