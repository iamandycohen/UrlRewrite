using Hi.UrlRewrite.Templates;
using Hi.UrlRewrite.Templates.Folders;
using Hi.UrlRewrite.Templates.Inbound;
using Sitecore.Data;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Events;
using Sitecore.SecurityModel;
using System;
using System.Linq;

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

                    if (redirectFolderItem == null) return;

                    if (item.IsRedirectFolderItem())
                    {
                        Log.Info(
                            string.Format("UrlRewrite - Refreshing Redirect Folder [{0}] after save event",
                                item.Paths.FullPath), this);

                        rulesEngine.GetCachedInboundRules(db);
                    }
                    else if (item.IsSimpleRedirectItem())
                    {
                        Log.Info(
                            string.Format("UrlRewrite - Refreshing Simple Redirect [{0}] after save event",
                                item.Paths.FullPath), this);

                        rulesEngine.RefreshSimpleRedirect(item, redirectFolderItem);
                    }
                    else if (item.IsInboundRuleItem())
                    {
                        Log.Info(
                            string.Format("UrlRewrite - Refreshing Inbound Rule [{0}] after save event",
                                item.Paths.FullPath), this);

                        rulesEngine.RefreshInboundRule(item, redirectFolderItem);
                    }
                    else if (item.IsRedirectType() && item.IsInboundRuleItemChild())
                    {
                        var inboundRuleItem = item.Parent;
                        var inboundRule = new InboundRuleItem(inboundRuleItem);

                        inboundRule.BeginEdit();
                        inboundRule.Action.InnerField.SetValue(item.ID.ToString(), false);
                        inboundRule.EndEdit();
                    }
                    else if (item.IsInboundRuleItemChild())
                    {
                        Log.Info(
                            string.Format("UrlRewrite - Refreshing Inbound Rule [{0}] after save event",
                                item.Parent.Paths.FullPath), this);

                        rulesEngine.RefreshInboundRule(item.Parent, redirectFolderItem);
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

            var rulesEngine = new RulesEngine();

            try
            {

                using (new SecurityDisabler())
                {

                    var redirectFolderItem = item.Axes.GetAncestors()
                        .FirstOrDefault(a => a.TemplateID == new ID(RedirectFolderItem.TemplateId));

                    if (redirectFolderItem != null)
                    {
                        if (item.IsInboundRuleItem() || item.IsSimpleRedirectItem())
                        {
                            Log.Info(
                                string.Format("UrlRewrite - Removing Inbound Rule [{0}] after delete event",
                                    item.Paths.FullPath), this);

                            rulesEngine.DeleteInboundRule(item, redirectFolderItem);
                        }
                        else if (item.IsInboundRuleItemChild())
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

    }
}
