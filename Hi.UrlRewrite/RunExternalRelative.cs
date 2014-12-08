using Sitecore.Shell.Framework;
using Sitecore.Shell.Framework.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.UrlRewrite
{
    public class RunExternalRelative : Command
    {

        public override void Execute(CommandContext context)
        {

            string urlFormat = "http://{0}/{1}";

            string url = context.Parameters["url"] ?? "";

            string param = context.Parameters["param"] ?? "";

            string icon = context.Parameters["icon"] ?? "";

            string title = context.Parameters["title"] ?? "";

            string hostName = Sitecore.Context.ClientPage.Request.Url.Host;

            if (!String.IsNullOrWhiteSpace(url) && !String.IsNullOrWhiteSpace(hostName))
            {

                url = String.Format(urlFormat, hostName, url);

                Windows.RunExternal(url, param, icon, title);

            }

        }

    }
}
