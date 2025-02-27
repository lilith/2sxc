﻿using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using ToSic.Razor.Markup;
using static ToSic.Sxc.Configuration.Features.BuiltInFeatures;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Images
{
    public class ResponsivePicture: ResponsiveBase, IResponsivePicture
    {
        internal ResponsivePicture(ImageService imgService, ResponsiveParams callParams, ILog parentLog) : base(imgService, callParams, parentLog, "Picture")
        {
        }


        public Picture Picture => _picTag.Get(() =>
        {
            var pic = Razor.Blade.Tag.Picture(Sources, Img);
            if (Call.PicClass.HasValue()) pic = pic.Class(Call.PicClass);
            return pic;
        });
        private readonly GetOnce<Picture> _picTag = new GetOnce<Picture>();

        protected override IHtmlTag GetOutermostTag() => Picture;

        public TagList Sources => _sourceTags.Get(() => SourceTagsInternal(Call.Link.Url, Settings));
        private readonly GetOnce<TagList> _sourceTags = new GetOnce<TagList>();

        private TagList SourceTagsInternal(string url, IResizeSettings resizeSettings)
        {
            var logOrNull = ImgService.Debug ? Log : null;
            var wrapLog = logOrNull.Fn<TagList>();
            // Check formats
            var defFormat = ImgService.GetFormat(url);
            if (defFormat == null) return wrapLog.Return(Razor.Blade.Tag.TagList(), "no format");

            // Determine if we have many formats, otherwise just use the current one
            var formats = defFormat.ResizeFormats.Any()
                ? defFormat.ResizeFormats
                : new List<IImageFormat> { defFormat };
            
            var useMultiSrcSet = ImgService.Features.IsEnabled(ImageServiceMultipleSizes.NameId);

            wrapLog.A($"{nameof(formats)}: {formats.Count}, {nameof(useMultiSrcSet)}: {useMultiSrcSet}");

            // Generate Meta Tags
            var sources = formats
                .Select(resizeFormat =>
                {
                    // We must copy the settings, because we change them and this shouldn't affect anything else
                    var formatSettings = new ResizeSettings(resizeSettings, format: resizeFormat != defFormat ? resizeFormat.Format : null);
                    var srcSet = useMultiSrcSet
                        ? ImgLinker.SrcSet(url, formatSettings, SrcSetType.Source, Call.HasDecoOrNull)
                        : ImgLinker.ImageOnly(url, formatSettings, Call.HasDecoOrNull).Url;
                    var source = Razor.Blade.Tag.Source().Type(resizeFormat.MimeType).Srcset(srcSet);
                    if (!string.IsNullOrEmpty(Sizes)) source.Sizes(Sizes);
                    return source;
                });
            var result = Razor.Blade.Tag.TagList(sources);
            return wrapLog.Return(result, $"{result.Count()}");
        }

    }
}
