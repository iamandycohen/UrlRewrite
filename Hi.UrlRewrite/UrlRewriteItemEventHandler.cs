using Sitecore.Data;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Events;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.UrlRewrite
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
            Item item;
            using (new SecurityDisabler())
            {
                var db = Database.GetDatabase(Configuration.Database);
                item = db.GetItem(itemId);
            }

            RunItemSaved(item);
        }

        private void RunItemSaved(Item item)
        {
            if (item.Database.Name.Equals(Configuration.Database, StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    using (new SecurityDisabler())
                    {
                        if (IsRedirectFolderItem(item))
                        {
                            Log.Info(string.Format("UrlRewrite - Refreshing Redirect Folder [{0}] after save event", item.Paths.FullPath), this);
                            UrlRewriteProcessor.RefreshInboundRulesCache();
                        }
                        else if (IsSimpleRedirectItem(item))
                        {
                            Log.Info(string.Format("UrlRewrite - Refreshing Simple Redirect [{0}] after save event", item.Paths.FullPath), this);
                            UrlRewriteProcessor.RefreshSimpleRedirect(item);
                        }
                        else if (IsInboundRuleItem(item))
                        {
                            Log.Info(string.Format("UrlRewrite - Refreshing Inbound Rule [{0}] after save event", item.Paths.FullPath), this);
                            UrlRewriteProcessor.RefreshInboundRule(item);
                        }
                        else if (IsInboundRuleItemChild(item))
                        {
                            Log.Info(string.Format("UrlRewrite - Refreshing Inbound Rule [{0}] after save event", item.Parent.Paths.FullPath), this);
                            UrlRewriteProcessor.RefreshInboundRule(item.Parent);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(string.Format("UrlRewrite - Exception occured when saving item after save - Item ID: {0} Item Path: {1}", item.ID, item.Paths.FullPath), ex, this);
                }

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
            if (item.Database.Name.Equals(Configuration.Database, StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {

                    using (new SecurityDisabler())
                    {
                        if (IsInboundRuleItem(item) || IsSimpleRedirectItem(item))
                        {
                            Log.Info(string.Format("UrlRewrite - Removing Inbound Rule [{0}] after delete event", item.Paths.FullPath), this);
                            UrlRewriteProcessor.DeleteInboundRule(item);
                        }
                        else if (IsInboundRuleItemChild(item))
                        {
                            Log.Info(string.Format("UrlRewrite - Removing Inbound Rule [{0}] after delete event", item.Parent.Paths.FullPath), this);
                            UrlRewriteProcessor.RefreshInboundRule(item.Parent);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(string.Format("UrlRewrite - Exception occured which deleting item after publish Item ID: {0} Item Path: {1}", item.ID, item.Paths.FullPath), ex, this);
                }
            }
        }

        #endregion

        #region Checks

        private static bool IsRedirectFolderItem(Item item)
        {
            return !IsTemplate(item) && item.TemplateID.ToString().Equals(Constants.RedirectFolderTemplateId, StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool IsSimpleRedirectItem(Item item)
        {
            return !IsTemplate(item) && item.TemplateID.ToString().Equals(Constants.SimpleRedirectInternalTemplateId, StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool IsInboundRuleItem(Item item)
        {
            return !IsTemplate(item) && item.TemplateID.ToString().Equals(Constants.InboundRuleTemplateId, StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool IsInboundRuleItemChild(Item item)
        {
            if (item.Parent != null)
            {
                return !IsTemplate(item) && item.Parent.TemplateID.ToString().Equals(Constants.InboundRuleTemplateId, StringComparison.InvariantCultureIgnoreCase);
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
