﻿using System.Collections.Generic;
using System.Linq;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Images
{
    [PrivateApi("Hide implementation")]
    public class ImageFormat : IImageFormat
    {
        /// <inheritdoc />
        public string Format { get; }

        /// <inheritdoc />
        public string MimeType { get; }

        /// <inheritdoc />
        public bool CanResize { get; }

        public IList<IImageFormat> ResizeFormats { get; }

        public ImageFormat(string format, string mimeType, bool canResize, IEnumerable<IImageFormat> better = null)
        {
            Format = format;
            MimeType = mimeType;
            CanResize = canResize;
            ResizeFormats = canResize
                ? better?.Union(new []{this}).ToList() ?? new List<IImageFormat> { this }
                : new List<IImageFormat>();
        }

        public ImageFormat(IImageFormat original, bool preserveSizes)
        {
            Format = original.Format;
            MimeType = original.MimeType;
            CanResize = original.CanResize;
            ResizeFormats = preserveSizes ? original.ResizeFormats : new List<IImageFormat>();
        }
    }
}
