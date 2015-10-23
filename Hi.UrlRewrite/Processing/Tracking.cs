using Hi.UrlRewrite.Processing.Results;
using Sitecore.Analytics;
using Sitecore.Analytics.Data;
using Sitecore.Analytics.Model;
using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Hi.UrlRewrite.Processing
{
    public class Tracking
    {
        private static Guid RedirectEventItemId = new Guid("{1d668f23-eeba-4bd3-93b3-94861ed42060}");

        private readonly static Tracking tracking = new Tracking();

        public static void TrackRedirect(ProcessInboundRulesResult results)
        {
            tracking.RegisterEventOnRedirect(results);
        }

        public void RegisterEventOnRedirect(ProcessInboundRulesResult results)
        {
            if (!Tracker.Enabled)
                return;

            if (!Tracker.IsActive)
                Tracker.StartTracking();

            try
            {

                foreach (var result in results.ProcessedResults.Where(e => e.RuleMatched))
                {
                    var itemId = result.ItemId;

                    var redirectItem = Sitecore.Context.Database.GetItem(new ID(itemId));

                    if (redirectItem != null)
                    {
                        var pageEventModel = new Sitecore.Analytics.Model.PageEventData()
                        {
                            PageEventDefinitionId = RedirectEventItemId,
                            ItemId = itemId,
                            Name = "UrlRewrite Redirect",
                            DateTime = DateTime.UtcNow,
                            Text = string.Format("Redirected from {0} to {1} using {2} [{3}].", result.OriginalUri, result.RewrittenUri, redirectItem.Name, itemId)
                        };

                        var pageEventData = new Sitecore.Analytics.Data.PageEventData(pageEventModel);

                        Tracker.Current.CurrentPage.Item = new Sitecore.Analytics.Model.ItemData
                        {
                            Id = itemId,
                            Language = redirectItem.Language.Name,
                            Version = redirectItem.Version.Number
                        };

                        Tracker.Current.CurrentPage.Register(pageEventData);
                        Tracker.Current.Interaction.AcceptModifications();
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Error(this, ex, "Exception occurred during tracking.");
            }
        }
    }
}