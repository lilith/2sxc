﻿using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using DotNetNuke.Framework;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Web.Client;
using DotNetNuke.Web.Client.ClientResourceManagement;
using DotNetNuke.Web.Client.Providers;
using ToSic.Eav;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Web.PageFeatures;
using ToSic.Sxc.Web.Url;

namespace ToSic.Sxc.Dnn.Web
{
    public class DnnClientResources: ServiceBase
    {
        /// <summary>
        /// DI Constructor
        /// </summary>
        public DnnClientResources(DnnJsApiHeader dnnJsApiHeader) : base($"{DnnConstants.LogName}.JsCss")
        {
            ConnectServices(Header = dnnJsApiHeader);
        }
        
        public DnnClientResources Init(Page page, bool? forcePre1025Behavior, IBlockBuilder blockBuilder)
        {
            _forcePre1025Behavior = forcePre1025Behavior;
            Page = page;
            BlockBuilder = blockBuilder as BlockBuilder;
            return this;
        }
        protected BlockBuilder BlockBuilder;
        protected Page Page;
        protected DnnJsApiHeader Header;
        private bool? _forcePre1025Behavior;


        internal IList<IPageFeature> Features => _features ?? (_features = BlockBuilder?.Run(true, null)?.Features ?? new List<IPageFeature>());
        private IList<IPageFeature> _features;

        public IList<IPageFeature> AddEverything(IList<IPageFeature> features = null)
        {
            var l = Log.Fn<IList<IPageFeature>>();
            // temporary solution, till the features are correctly activated in the block
            // auto-detect Blockbuilder params
            features = features ?? Features;

            // normal scripts
            var editJs = features.Contains(BuiltInFeatures.JsCmsInternal);
            var readJs = features.Contains(BuiltInFeatures.JsCore);
            var editCss = features.Contains(BuiltInFeatures.ToolbarsInternal);

            if (!readJs && !editJs && !editCss && !features.Any())
                return l.Return(features, "nothing to add");

            l.A("user is editor, or template requested js/css, will add client material");

            // register scripts and css
            RegisterClientDependencies(Page, readJs, editJs, editCss, features);

            // New in 11.11.02 - DNN has a strange behavior where the current language isn't known till PreRender
            // so we have to move adding the header to here.
            // MustAddHeaders may have been set earlier by the engine, or now by the various js added
            l.A($"{nameof(MustAddHeaders)}={MustAddHeaders}");
            if (MustAddHeaders) Header.AddHeaders();

            return l.ReturnAsOk(features);
        }


        /// <summary>
        /// new in 10.25 - by default jQuery isn't loaded any more
        /// but older razor templates might still expect it
        /// and any other old behaviour, incl. no-view defined, etc. should activate compatibility
        /// </summary>
        public void EnforcePre1025Behavior() => Log.Do(message: "Activate Anti-Forgery for compatibility with old behavior", action: () =>
        {
            ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
            MustAddHeaders = true;
        });

        /// <summary>
        /// new in 10.25 - by default now jQuery isn't loaded!
        /// but any old behaviour, incl. no-view defined, etc. should activate compatibility
        /// </summary>
        /// <returns></returns>
        public bool NeedsPre1025Behavior() => _forcePre1025Behavior
                                              ?? BlockBuilder?.GetEngine()?.CompatibilityAutoLoadJQueryAndRvt
                                              ?? true;


        public void RegisterClientDependencies(Page page, bool readJs, bool editJs, bool editCss, IList<IPageFeature> overrideFeatures = null) =>
            Log.Do($"-, {nameof(readJs)}:{readJs}, {nameof(editJs)}:{editJs}, {nameof(editCss)}:{editCss}", l =>
            {
                var features = overrideFeatures ?? Features;

                var root = DnnConstants.SysFolderRootVirtual;
                root = page.ResolveUrl(root);
                var ver = EavSystemInfo.VersionWithStartUpBuild;
                var priority = (int)FileOrder.Js.DefaultPriority - 2;

                // add edit-mode CSS
                if (editCss) RegisterCss(page, $"{root}{BuiltInFeatures.ToolbarsInternal.UrlWip}");

                // add read-js
                if (readJs || editJs)
                {
                    l.A("add $2sxc api and headers");
                    RegisterJs(page, ver, $"{root}{BuiltInFeatures.JsCore.UrlWip}", true, priority);
                    MustAddHeaders = true;
                }

                // add edit-js (commands, manage, etc.)
                if (editJs)
                {
                    l.A("add 2sxc edit api; also needs anti-forgery");
                    // note: the inpage only works if it's not in the head, so we're adding it below
                    RegisterJs(page, ver, $"{root}{BuiltInFeatures.JsCmsInternal.UrlWip}", false, priority + 1);
                }

                if (features.Contains(BuiltInFeatures.JQuery))
                    JavaScript.RequestRegistration(CommonJs.jQuery);

                if (features.Contains(BuiltInFeatures.TurnOn))
                    RegisterJs(page, ver, $"{root}{BuiltInFeatures.TurnOn.UrlWip}", true, priority + 10);

                if (features.Contains(BuiltInFeatures.CmsWysiwyg))
                    RegisterCss(page, $"{root}{BuiltInFeatures.CmsWysiwyg.UrlWip}");
            });


        #region DNN Bug with Current Culture

        // We must add the _js header but we must wait beyond the initial page-load until Pre-Render
        // Because for reasons unknown DNN (at least in V7.4+ but I think also in 9) doesn't have 
        // the right PortalAlias and language set until then. 
        // before that it assumes the PortalAlias is a the default alias, even if the url clearly shows another language

        private bool MustAddHeaders { get; set; }

        #endregion


        #region add scripts / css with bypassing the official ClientResourceManager

        private static void RegisterJs(Page page, string version, string path, bool toHead, int priority)
        {
            if (string.IsNullOrWhiteSpace(path)) return;

            var url = UrlHelpers.QuickAddUrlParameter(path, "v", version);
            if (toHead)
                ClientResourceManager.RegisterScript(page, url, priority, DnnPageHeaderProvider.DefaultName);
            else
                page.ClientScript.RegisterClientScriptInclude(typeof(Page), path, url);
        }

        private static void RegisterCss(Page page, string path)
            => ClientResourceManager.RegisterStyleSheet(page, path);

        #endregion



    }
}
