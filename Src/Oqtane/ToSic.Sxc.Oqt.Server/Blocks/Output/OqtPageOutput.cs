﻿using Oqtane.Shared;
using System;
using System.Collections.Generic;
using ToSic.Eav.Helpers;
using ToSic.Lib.Documentation;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Output;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web.PageFeatures;

namespace ToSic.Sxc.Oqt.Server.Blocks.Output
{
    [PrivateApi]
    public partial class OqtPageOutput : ServiceBase
    {
        #region Constructor and DI

        public OqtPageOutput(SiteState siteState, IBlockResourceExtractor blockResourceExtractor, IJsApiService jsApiService) : base($"{OqtConstants.OqtLogPrefix}.AssHdr")
        {
            ConnectServices(
                _siteState = siteState,
                _blockResourceExtractor = blockResourceExtractor,
                _jsApiService = jsApiService
            );
        }

        private readonly SiteState _siteState;
        private readonly IBlockResourceExtractor _blockResourceExtractor;
        private readonly IJsApiService _jsApiService;
        
        public void Init(OqtSxcViewBuilder parent, IRenderResult renderResult)
        {
            Parent = parent;
            RenderResult = renderResult;
        }

        protected OqtSxcViewBuilder Parent;
        protected IRenderResult RenderResult;

        #endregion


        private bool AddJsCore => Features.Contains(BuiltInFeatures.JsCore);
        private bool AddJsEdit => Features.Contains(BuiltInFeatures.JsCmsInternal);


        /// <summary>
        /// The JavaScripts needed
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> Scripts()
        {
            var list = new List<string>();

            // v12.03, Oqtane 2.2 with Bootstrap 5 do not includes jQuery any more
            // as Oqtane 2.1 with Bootstrap 4
            if (Features.Contains(BuiltInFeatures.JQuery)) 
                list.Add("//code.jquery.com/jquery-3.5.1.min.js");

            if (AddJsCore) list.Add($"{OqtConstants.UiRoot}/{BuiltInFeatures.JsCore.UrlWip}");

            if (AddJsEdit) list.Add($"{OqtConstants.UiRoot}/{BuiltInFeatures.JsCmsInternal.UrlWip}");

            // New in 12.02
            if (Features.Contains(BuiltInFeatures.TurnOn))
                list.Add($"{OqtConstants.UiRoot}/{BuiltInFeatures.TurnOn.UrlWip}");

            
            return list;
        }

        /// <summary>
        /// The styles to add
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> Styles()
        {
            var list = new List<string>();
            if (Features.Contains(BuiltInFeatures.ToolbarsInternal))
                list.Add($"{OqtConstants.UiRoot}/{BuiltInFeatures.ToolbarsInternal.UrlWip}");

            // New 15.01
            if (Features.Contains(BuiltInFeatures.CmsWysiwyg))
                list.Add($"{OqtConstants.UiRoot}/{BuiltInFeatures.CmsWysiwyg.UrlWip}");
            return list;
        }

        [PrivateApi]
        public static string GetSiteRoot(SiteState siteState)
            => siteState?.Alias?.Name == null ? OqtConstants.SiteRoot : new Uri($"http://{siteState.Alias.Name}/").AbsolutePath.SuffixSlash();

        internal IList<IPageFeature> Features => _features ??= RenderResult.Features ?? new List<IPageFeature>();
        private IList<IPageFeature> _features;

    }
}
