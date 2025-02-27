﻿using ToSic.Lib.Helpers;
using ToSic.Sxc.Web.ClientAssets;
using ToSic.Sxc.Web.PageService;

namespace ToSic.Sxc.Blocks.Output
{
    /// <summary>
    /// ATM only used in Oqtane, where external and internal scripts must be extracted
    /// </summary>
    public class BlockResourceExtractorWithInline: BlockResourceExtractor
    {
        public BlockResourceExtractorWithInline(PageServiceShared pageServiceShared): base(pageServiceShared) { }

        protected override ClientAssetsExtractSettings Settings => _settings.Get(() => new ClientAssetsExtractSettings(extractAll: true));
        private readonly GetOnce<ClientAssetsExtractSettings> _settings = new GetOnce<ClientAssetsExtractSettings>();

        protected override (string Template, bool Include2sxcJs) ExtractFromHtml(string html, ClientAssetsExtractSettings settings)
        {
            var include2SxcJs = false;

            // Handle Client Dependency injection
            html = ExtractExternalScripts(html, ref include2SxcJs, settings);

            // Handle inline JS
            html = ExtractInlineScripts(html);

            // Handle Styles
            html = ExtractStyles(html, settings);

            Assets.ForEach(a => a.PosInPage = PositionNameUnchanged(a.PosInPage));

            return (html, include2SxcJs);
        }



        private string PositionNameUnchanged(string position)
        {
            position = position.ToLowerInvariant();

            switch (position)
            {
                case "body":
                case "head":
                    return position;
                default:
                    return "body";
            }
        }

    }

}
