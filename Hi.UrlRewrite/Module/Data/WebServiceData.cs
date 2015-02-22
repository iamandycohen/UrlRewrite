using Hi.UrlRewrite.ScWebService;
using Sitecore;
using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Xml.Linq;

namespace Hi.UrlRewrite.Data
{
    public class WebServiceData
    {
        private const string baseAddreess = "/sitecore/shell/webservice/service.asmx";
        private readonly Database db;
        private readonly string baseHost;

        public WebServiceData(string host) : this(Context.Database, host)
        {
        }

        public WebServiceData(Database db, string host)
        {
            this.db = db;
            baseHost = host;
        }

        public XElement GetItem(ID id)
        {
            var binding = new BasicHttpBinding();
            var address = new Uri(new Uri("http://" + baseHost), baseAddreess).ToString();
            var endpoint = new EndpointAddress(address);
            var client = new ScWebService.VisualSitecoreServiceSoapClient(binding, endpoint);
            var credentials = new Credentials() { UserName = @"sitecore\admin", Password = "b" };
            return client.GetXML(id.ToString(), false, db.Name, credentials);


        }
    }
}