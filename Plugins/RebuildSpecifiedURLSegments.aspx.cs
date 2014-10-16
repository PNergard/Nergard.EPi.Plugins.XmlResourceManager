using System;
using System.Collections.Generic;
using System.Web.Security;
using System.Web.UI.WebControls;
using EPiServer.Personalization;
using EPiServer.PlugIn;
using EPiServer.Security;
using EPiServer.Util.PlugIns;
using System.Web.UI;
using EPiServer.Shell.WebForms;
using log4net;
using EPiServer.UI.Admin;
using EPiServer.Core;
using EPiServer;
using EPiServer.Web;
using EPiServer.DataAccess;
using System.Threading;
using EPiServer.ClientScript;

namespace Nergard.EPi.Plugins.RebuildSpecifiedURLSegments.Plugins
{
    [GuiPlugIn(DisplayName = "RebuildSpecifiedURLSegments", Description = "", Area = PlugInArea.AdminMenu, Url = "~/Plugins/RebuildSpecifiedURLSegments.aspx")]
    public partial class RebuildSpecifiedURLSegments : WebFormsBase
    {
        private static readonly ILog _log;
        private static string _error;
        private static bool _rebuildNotEmpty;
        private static int _scanedPages;
        private static int _convertedPages;
        private static Thread _mainThread;


        static RebuildSpecifiedURLSegments()
        {
            _log = LogManager.GetLogger(typeof(RebuildSpecifiedURLSegments));
            _error = string.Empty;
            _rebuildNotEmpty = true;
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            this.MasterPageFile = UriSupport.ResolveUrlFromUIBySettings("MasterPages/EPiServerUI.master");
            this.SystemMessageContainer.Heading = "RebuildSpecifiedURLSegments";
            this.SystemMessageContainer.Description = "Rebuild URLSegments starting from a specified page.";
        }

        protected override void OnLoad(EventArgs e)
        {
            SetStatusLabel();
        }

        protected void ButtonRebuild_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtReference.Text))
                if ((_mainThread == null) || !_mainThread.IsAlive)
                {
                    _convertedPages = 0;
                    _scanedPages = 0;
                    ThreadStart start = new ThreadStart(() => this.ConvertRecursive(PageReference.Parse(txtReference.Text)));
                    _mainThread = new Thread(start);
                    _mainThread.Start();
                }

            base.ScriptManager.AddEventListener(this, EventType.Load, "function(){setTimeout(function(){document.location.href=document.location.href},1000)}");
        }

        private void ConvertRecursive(PageReference pageId)
        {
            IEnumerable<PageData> languageBranches = this.ContentRepository.GetLanguageBranches<PageData>(pageId);
            bool flag = false;
            foreach (PageData data in languageBranches)
            {
                try
                {
                    if (_rebuildNotEmpty && !this.IsSystemPage(data))
                    {
                        string uRLSegment = data.URLSegment;
                        PageData content = data.CreateWritableClone();
                        if (_rebuildNotEmpty)
                        {
                            content.URLSegment = string.Empty;
                        }
                        string uniqueURLSegment = UrlSegment.GetUniqueURLSegment(content);
                        if (string.IsNullOrEmpty(uRLSegment) || (string.Compare(uRLSegment, uniqueURLSegment) != 0))
                        {
                            PageData data3 = this.ContentRepository.Get<PageData>(new PageReference(data.PageLink.ID, 0, data.PageLink.ProviderName, true), new LanguageSelector(data.LanguageBranch)).CreateWritableClone();
                            data3["PageURLSegment"] = uniqueURLSegment;
                            if (data3.IsPendingPublish)
                            {
                                this.ContentRepository.Save(data3, SaveAction.ForceCurrentVersion | SaveAction.Save, AccessLevel.NoAccess);
                            }
                            else
                            {
                                this.ContentRepository.Save(data3, SaveAction.ForceCurrentVersion | SaveAction.Publish, AccessLevel.NoAccess);
                            }
                            flag = true;
                            if (_log.IsDebugEnabled)
                            {
                                _log.Debug(string.Concat(new object[] { "Setting URL segment for page '", data.PageLink, "' on language '", data.LanguageBranch, "' from '", uRLSegment, "' to '", uniqueURLSegment, "'" }));
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    _log.Error(string.Concat(new object[] { "Can't set URL segment for page '", data.PageLink, "' on language '", data.LanguageBranch, "' because:" }), exception);
                }
            }
            if (flag)
            {
                _convertedPages++;
            }
            _scanedPages++;
            foreach (PageData data4 in this.ContentRepository.GetChildren<PageData>(pageId, LanguageSelector.AutoDetect(true)))
            {
                this.ConvertRecursive(data4.PageLink);
            }
            DataFactoryCache.RemoveListing(pageId);
        }

        private bool IsSystemPage(PageData page)
        {
            if (!page.PageLink.CompareToIgnoreWorkID(ContentReference.RootPage) && !page.PageLink.CompareToIgnoreWorkID(ContentReference.WasteBasket))
            {
                return false;
            }
            return true;
        }

        private void SetStatusLabel()
        {
            this.LabelStatusPages.Text = _scanedPages.ToString();
            this.LabelPagesToReplace.Text = _convertedPages.ToString();
            base.SystemMessageContainer.Description = this.Translate("/admin/rebuildurlsegments/description");
            if ((_mainThread == null) || !_mainThread.IsAlive)
            {
                this.LabelStatus.Text = this.Translate("/admin/rebuildurlsegments/statusended");

                _convertedPages = 0;
                _scanedPages = 0;
                _mainThread = null;
                if (_error.Length > 0)
                {
                    base.SystemMessageContainer.SetWarning(_error);
                    _error = string.Empty;
                }
            }
            else
            {
                this.LabelStatus.Text = this.Translate("/admin/rebuildurlsegments/statusstarted");
                base.ScriptManager.AddEventListener(this, EventType.Load, "function(){setTimeout(function(){document.location.href=document.location.href},1000)}");
            }
        }





 

 

    }
}