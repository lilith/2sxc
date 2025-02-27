﻿using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Metadata;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Context
{
    [PrivateApi("hide implementation")]
    public class CmsPage: CmsContextPartBase<IPage>, ICmsPage
    {
        public CmsPage(CmsContext parent, AppState appState, LazySvc<IPage> fallbackPage)
            : base(parent, parent?.CtxBlockOrNull?.Page ?? fallbackPage.Value)
        {
            _appState = appState;
        }

        private readonly AppState _appState;

        public int Id => GetContents()?.Id ?? 0;
        public IParameters Parameters => GetContents()?.Parameters;
        public string Url => GetContents().Url ?? string.Empty;

        protected override IMetadataOf GetMetadataOf()
        {
            var md = _appState.GetMetadataOf(TargetTypes.Page, Id, Url);
            if (md == null) return null;
            md.Target.Recommendations = new[] { Decorators.NoteDecoratorName, Decorators.OpenGraphName };
            return md;
        }
    }
}
