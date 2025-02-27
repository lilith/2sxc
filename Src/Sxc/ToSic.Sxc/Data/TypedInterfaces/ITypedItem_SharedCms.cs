﻿using ToSic.Razor.Blade;
using ToSic.Sxc.Images;
using ToSic.Sxc.Services;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data
{
    public partial interface ITypedItem
    {
        /// <summary>
        /// Many templates show demo data.
        /// If the template code must know if it's the demo item or
        /// real data, use `.IsDemoItem`.
        /// </summary>
        /// <returns>
        /// True if this is the item configured in the view-settings, false if not.
        /// </returns>
        bool IsDemoItem { get; }

        /// <summary>
        /// Show a field in the expected / best possible way.
        /// As of now it's meant for WYSIWYG fields with Very-Rich Text.
        /// See [](xref:NetCode.DynamicData.DynamicEntityHtml)
        /// </summary>
        /// <param name="name">the field name</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="container">
        /// A wrapper tag for the result.
        /// It's either a RazorBlade tag such as `Kit.HtmlTag.Div()`, a string such as `span` or an empty string `` to indicate no container.
        /// If not set it will default to to a div-tag.
        /// See [docs](xref:NetCode.DynamicData.DynamicEntityHtml)
        /// </param>
        /// <param name="toolbar">Override default toolbar behavior on this field. See [docs](xref:NetCode.DynamicData.DynamicEntityHtml)</param>
        /// <param name="imageSettings">Settings for resizing. Default is `Wysiwyg` but it can also be `Content` or a settings object.</param>
        /// <param name="required">throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired)</param>
        /// <param name="debug">Activate debug visualization to better see alignments and such.</param>
        /// <returns></returns>
        /// <remarks>
        /// * Added in 2sxc 16.01
        /// * Only works on Razor files inheriting from Hybrid14 or newer
        /// </remarks>
        IHtmlTag Html(
            string name,
            string noParamOrder = Protector,
            object container = default,
            bool? toolbar = default,
            object imageSettings = default,
            bool? required = default,
            bool debug = default
        );


        /// <summary>
        /// Get a Responsive Picture object which you can then either just show, or use to construct a more customized output as you need it.
        /// 
        /// The resulting object can just be added to the html, like `@pic` or you can work with sub-properties as specified in the <see cref="IResponsivePicture"/>.
        /// 
        /// **Important:** This call only allows you to set the most common parameters `factor` and `width`.
        /// For other parameters like `height`, `aspectRatio`, `quality` etc. create typed Settings <see cref="IImageService.Settings"/> and pass them in.
        ///
        /// > [!NOTE]
        /// > This is the similar as using the <see cref="IImageService.Picture"/> just a bit simpler.
        /// >
        /// > An important difference is that it returns `null` if the field does not exist or is empty, allowing you to just show nothing or use `...Picture(...) ?? someFallback;`
        /// </summary>
        /// <param name="name">Name of a field</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="settings">
        /// - The name of a settings configuration, like "Content", "Screen", "Square", etc.
        /// - A standardized Image-Settings object like Settings.Child("Images.Content") - see https://go.2sxc.org/settings
        /// - A dynamic object containing settings properties (this can also be a merged custom + standard settings)
        /// - A strictly typed <see cref="IResizeSettings"/> object containing all settings created using <see cref="ToSic.Sxc.Services.IImageService.Settings">ResizeSettings</see> 
        /// </param>
        /// <param name="factor">An optional multiplier, usually used to create urls which resize to a part of the default content-size. Eg. 0.5. </param>
        /// <param name="width">An optional, fixed width of the image</param>
        /// <param name="imgAlt">
        /// Optional `alt` attribute on the created `img` tag for SEO etc.
        /// If supplied, it takes precedence to the alt-description in the image metadata which the editor added themselves.
        /// If you want to provide a fallback value (in case the metadata has no alt), use `imgAltFallback`
        /// </param>
        /// <param name="imgAltFallback">
        /// Optional `alt` attribute which is only used if the `imgAlt` or the alt-text in the metadata are empty.
        /// </param>
        /// <param name="imgClass">Optional `class` attribute on the created `img` tag</param>
        /// <param name="required">throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired)</param>
        /// <param name="recipe">
        /// Optional recipe = instructions how to create the various variants of this link.
        /// Can be any one of these:
        /// 
        /// - string containing variants
        /// - Rule object
        /// 
        /// TODO: DOCS not quite ready
        /// </param>
        /// <returns>
        /// * A <see cref="IResponsivePicture"/> object which can be rendered directly. See [](xref:NetCode.Images.Index)
        /// * If the field does not exist, it will return `null`
        /// * If the field exists, but is empty, it will return `null`
        /// </returns>
        /// <remarks>
        /// * Added to ITypedItem in v16.03
        /// * We have this `Picture` method but no `Img` method, as we don't recommended that in responsive HTML5
        /// </remarks>
        IResponsivePicture Picture(
            string name,
            string noParamOrder = Protector,
            object settings = default,
            object factor = default,
            object width = default,
            string imgAlt = default,
            string imgAltFallback = default,
            string imgClass = default,
            object recipe = default
        );
    }
}
