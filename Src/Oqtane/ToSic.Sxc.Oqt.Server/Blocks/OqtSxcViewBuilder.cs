﻿using System.Collections.Generic;
using Oqtane.Models;
using System.Linq;
using ToSic.Lib.Logging;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Oqt.Server.Installation;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Oqt.Shared.Models;
using ToSic.Sxc.Web.LightSpeed;
using ToSic.Sxc.Web.Url;
using Page = Oqtane.Models.Page;
using ToSic.Sxc.Oqt.Server.Services;

namespace ToSic.Sxc.Oqt.Server.Blocks
{
    [PrivateApi]
    public class OqtSxcViewBuilder : ServiceBase
    {
        #region Constructor and DI

        public OqtSxcViewBuilder(
            Output.OqtPageOutput pageOutput,
            IContextOfBlock contextOfBlockEmpty,
            BlockFromModule blockModuleEmpty,
            IContextResolver contextResolverForLookUps,
            ILogStore logStore,
            GlobalTypesCheck globalTypesCheck,
            OqtPrerenderService oqtPrerenderService,
            IOutputCache outputCache
        ) : base($"{OqtConstants.OqtLogPrefix}.Buildr")
        {

            ConnectServices(
                _contextOfBlockEmpty = contextOfBlockEmpty,
                _blockModuleEmpty = blockModuleEmpty,
                _contextResolverForLookUps = contextResolverForLookUps,
                _globalTypesCheck = globalTypesCheck,
                _oqtPrerenderService = oqtPrerenderService,
                _outputCache = outputCache,
                PageOutput = pageOutput
            );
            logStore.Add("oqt-view", Log);
        }

        public Output.OqtPageOutput PageOutput { get; }
        private readonly IContextOfBlock _contextOfBlockEmpty;
        private readonly BlockFromModule _blockModuleEmpty;
        private readonly IContextResolver _contextResolverForLookUps;
        private readonly GlobalTypesCheck _globalTypesCheck;
        private readonly OqtPrerenderService _oqtPrerenderService;
        private readonly IOutputCache _outputCache;

        #endregion

        #region Prepare

        /// <summary>
        /// Prepare must always be the first thing to be called - to ensure that afterwards both headers and html are known.
        /// </summary>
        public OqtViewResultsDto Prepare(Alias alias, Site site, Page page, Module module, bool preRender)
        {
            Alias = alias;
            Site = site;
            Page = page;
            Module = module;
            PreRender = preRender;

            // Check for installation errors before even trying to build a view, and otherwise return this object if Refs are missing.
            if (RefsInstalledCheck.WarnIfRefsAreNotInstalled(out var oqtViewResultsDtoWarning)) return oqtViewResultsDtoWarning;

            OqtViewResultsDto ret = null;
            var finalMessage = "";
            LogTimer.DoInTimer(() => Log.Do(timer: true, action: () =>
            {
                #region Lightspeed output caching
                if (OutputCache?.Existing != null) Log.A("Lightspeed hit - will use cached");
                var renderResult = OutputCache?.Existing?.Data ?? Block.BlockBuilder.Run(true, null);
                finalMessage = OutputCache?.IsEnabled != true ? "" :
                    OutputCache?.Existing?.Data != null ? "⚡⚡" : "⚡⏳";
                OutputCache?.Save(renderResult);

                #endregion

                PageOutput.Init(this, renderResult);

                ret = new()
                {
                    Html = renderResult.Html,
                    TemplateResources = PageOutput.GetSxcResources(),
                    SxcContextMetaName = PageOutput.AddContextMeta ? PageOutput.ContextMetaName : null,
                    SxcContextMetaContents = PageOutput.AddContextMeta ? PageOutput.ContextMetaContents() : null,
                    SxcScripts = PageOutput.Scripts().ToList(),
                    SxcStyles = PageOutput.Styles().ToList(),
                    PageProperties = PageOutput.GetOqtPagePropertyChangesList(renderResult.PageChanges),
                    HttpHeaders = ConvertHttpHeaders(renderResult.HttpHeaders),
                    CspEnabled = renderResult.CspEnabled,
                    CspEnforced = renderResult.CspEnforced,
                    CspParameters = renderResult.CspParameters.Select(c => c.NvcToString())
                        .ToList(), // convert NameValueCollection to (query) string because can't serialize NameValueCollection to json
                    SystemHtml = PreRender ? _oqtPrerenderService?.GetSystemHtml() : string.Empty
            };
            }));
            LogTimer.Done(OutputCache?.Existing?.Data?.IsError ?? false ? "⚠️" : finalMessage);

            // Check if there is less than 50 global types and warn user to restart application
            // HACK: in v14.03 this check was moved bellow LogTimer.DoInTimer because we got exception (probably timing issue)
            // "Object reference not set to an instance of an object. at ToSic.Eav.Apps.AppStates.Get(IAppIdentity app)"
            // TODO: STV find correct fix
            if (_globalTypesCheck.WarnIfGlobalTypesAreNotLoaded(out var oqtViewResultsDtoWarning2)) return oqtViewResultsDtoWarning2;

            return ret;
        }

        // convert System.Collections.Generic.IList<ToSic.Sxc.Web.PageService.HttpHeader> to System.Collections.Generic.IList<ToSic.Sxc.Oqt.Shared.HttpHeader>
        private static IList<HttpHeader> ConvertHttpHeaders(IList<Web.PageService.HttpHeader> httpHeaders) 
            => httpHeaders.Select(httpHeader => new HttpHeader(httpHeader.Name, httpHeader.Value)).ToList();

        internal Alias Alias;
        internal Site Site;
        internal Page Page;
        internal Module Module;
        internal bool PreRender;

        internal IBlock Block => _blockGetOnce.Get(() => LogTimer.DoInTimer(() =>
        {
            var ctx = _contextOfBlockEmpty.Init(Page.PageId, Module);
            var block = _blockModuleEmpty.Init(ctx);
            var blcWithCtx = new BlockWithContextProvider(ctx, () => block);
            // Special for Oqtane - normally the IContextResolver is only used in WebAPIs
            // But the ModuleLookUp and PageLookUp also rely on this, so the IContextResolver must know about this for now
            // In future, we should find a better way for this, so that IContextResolver is really only used on WebApis
            _contextResolverForLookUps.AttachBlock(blcWithCtx);
            return block;
        }));
        private readonly GetOnce<IBlock> _blockGetOnce = new();

        protected ILogCall LogTimer => _logTimer.Get(() => Log.Fn(message: $"PreRender:{PreRender}, Page:{Page?.PageId} '{Page?.Name}', Module:{Module?.ModuleId} '{Module?.Title}'"));
        private readonly GetOnce<ILogCall> _logTimer = new();


        protected IOutputCache OutputCache => _oc.Get(() => _outputCache.Init(Module.ModuleId, Page?.PageId ?? 0, Block));
        private readonly GetOnce<IOutputCache> _oc = new();

        #endregion
    }
}
