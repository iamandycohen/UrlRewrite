using System;
using System.Linq;
using Hi.UrlRewrite.Templates;
using Sitecore.Data;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Events;
using Sitecore.SecurityModel;

namespace Hi.UrlRewrite.Processing
{
    public class UrlRewriteItemEventHandler
    {

        #region ItemSaved

        public void OnItemSaved(object sender, EventArgs args)
        {
            var item = Event.ExtractParameter(args, 0) as Item;
            if (item == null)
            {
                return;
            }

            RunItemSaved(item);
        }

        public void OnItemSavedRemote(object sender, EventArgs args)
        {
            var itemSavedRemoteEventArg = args as ItemSavedRemoteEventArgs;
            if (itemSavedRemoteEventArg == null)
            {
                return;
            }

            var itemId = itemSavedRemoteEventArg.Item.ID;
            var db = itemSavedRemoteEventArg.Item.Database;
            Item item;

            using (new SecurityDisabler())
            {
                //var db = Database.GetDatabase(Configuration.Database);
                item = db.GetItem(itemId);
            }

            RunItemSaved(item);
        }

        private void RunItemSaved(Item item)
        {
            var db = item.Database;
            var rulesEngine = new RulesEngine();

            try
            {
                using (new SecurityDisabler())
                {
                    var redirectFolderItem = item.Axes.GetAncestors()
                        .FirstOrDefault(a => a.TemplateID == new ID(RedirectFolderItem.TemplateId));

                    if (redirectFolderItem != null)
                    {

                        if (IsRedirectFolderItem(item))
                        {
                            Log.Info(
                                string.Format("UrlRewrite - Refreshing Redirect Folder [{0}] after save event",
                                    item.Paths.FullPath), this);

                            rulesEngine.RefreshInboundRulesCache(db);
                        }
                        else if (IsSimpleRedirectItem(item))
                        {
                            Log.Info(
                                string.Format("UrlRewrite - Refreshing Simple Redirect [{0}] after save event",
                                    item.Paths.FullPath), this);

                            rulesEngine.RefreshSimpleRedirect(item, redirectFolderItem);
                        }
                        else if (IsInboundRuleItem(item))
                        {
                            Log.Info(
                                string.Format("UrlRewrite - Refreshing Inbound Rule [{0}] after save event",
                                    item.Paths.FullPath), this);

                            rulesEngine.RefreshInboundRule(item, redirectFolderItem);
                        }
                        else if (IsInboundRuleItemChild(item))
                        {
                            Log.Info(
                                string.Format("UrlRewrite - Refreshing Inbound Rule [{0}] after save event",
                                    item.Parent.Paths.FullPath), this);

                            rulesEngine.RefreshInboundRule(item.Parent, redirectFolderItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("UrlRewrite - Exception occured when saving item after save - Item ID: {0} Item Path: {1}", item.ID, item.Paths.FullPath), ex, this);
            }

        }

        #endregion

        #region ItemDeleted

        public void OnItemDeleted(object sender, EventArgs args)
        {
            var item = Event.ExtractParameter(args, 0) as Item;
            if (item == null)
            {
                return;
            }

            RunItemDeleted(item);
        }

        public void OnItemDeletedRemote(object sender, EventArgs args)
        {
            var itemDeletedRemoteEventArg = args as ItemDeletedRemoteEventArgs;
            if (itemDeletedRemoteEventArg == null)
            {
                return;
            }

            RunItemDeleted(itemDeletedRemoteEventArg.Item);
        }

        private void RunItemDeleted(Item item)
        {
            //if (item.Database.Name.Equals(Configuration.Database, StringComparison.InvariantCultureIgnoreCase))
            //{

            var rulesEngine = new RulesEngine();

            try
            {

                using (new SecurityDisabler())
                {

                    var redirectFolderItem = item.Axes.GetAncestors()
                        .FirstOrDefault(a => a.TemplateID == new ID(RedirectFolderItem.TemplateId));

                    if (redirectFolderItem != null)
                    {
                        if (IsInboundRuleItem(item) || IsSimpleRedirectItem(item))
                        {
                            Log.Info(
                                string.Format("UrlRewrite - Removing Inbound Rule [{0}] after delete event",
                                    item.Paths.FullPath), this);

                            rulesEngine.DeleteInboundRule(item, redirectFolderItem);
                        }
                        else if (IsInboundRuleItemChild(item))
                        {
                            Log.Info(
                                string.Format("UrlRewrite - Removing Inbound Rule [{0}] after delete event",
                                    item.Parent.Paths.FullPath), this);

                            rulesEngine.RefreshInboundRule(item.Parent, redirectFolderItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("UrlRewrite - Exception occured which deleting item after publish Item ID: {0} Item Path: {1}", item.ID, item.Paths.FullPath), ex, this);
            }
        }

        #endregion

        #region Checks

        private static bool IsRedirectFolderItem(Item item)
        {
            return !IsTemplate(item) && item.TemplateID.ToString().Equals(RedirectFolderItem.TemplateId, StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool IsSimpleRedirectItem(Item item)
        {
            return !IsTemplate(item) && item.TemplateID.ToString().Equals(SimpleRedirectItem.TemplateId, StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool IsInboundRuleItem(Item item)
        {
            return !IsTemplate(item) && item.TemplateID.ToString().Equals(InboundRuleItem.TemplateId, StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool IsInboundRuleItemChild(Item item)
        {
            if (item.Parent != null)
            {
                return !IsTemplate(item) && item.Parent.TemplateID.ToString().Equals(InboundRuleItem.TemplateId, StringComparison.InvariantCultureIgnoreCase);
            }
            return false;
        }

        private static bool IsTemplate(Item item)
        {
            return item.Paths.FullPath.StartsWith("/sitecore/templates", StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion

    }
}
