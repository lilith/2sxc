﻿using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Metadata;
using ToSic.Lib;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using App = ToSic.Sxc.Apps.App;
using IApp = ToSic.Sxc.Apps.IApp;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Context
{
    [PrivateApi("Hide implementation")]
    public class CmsSite: CmsContextPartBase<ISite>, ICmsSite
    {
        public CmsSite(LazySvc<App> siteAppLazy) => _siteAppLazy = siteAppLazy;
        private readonly LazySvc<App> _siteAppLazy;

        public ICmsSite Init(CmsContext parent, AppState appState)
        {
            base.Init(parent, parent.CtxSite.Site);
            _appState = appState;
            return this;
        }

        private AppState _appState;

        public int Id => GetContents()?.Id ?? Eav.Constants.NullId;
        public string Url => GetContents()?.Url ?? string.Empty;
        public string UrlRoot => GetContents().UrlRoot ?? string.Empty;

        public IApp App => _app.Get(() => _siteAppLazy.Value.Init(_appState, null));
        private readonly GetOnce<IApp> _app = new GetOnce<IApp>();

        protected override IMetadataOf GetMetadataOf() 
            => ExtendWithRecommendations(_appState.GetMetadataOf(TargetTypes.Site, Id, Url));
    }
}
