﻿using ToSic.Razor.Blade;

namespace ToSic.Sxc.Web.PageService
{
    public partial class PageService
    {

        /// <inheritdoc />
        public string AddToHead(IHtmlTag tag)
        {
            PageServiceShared.Add(tag);
            return "";
        }

        /// <inheritdoc />
        public string AddToHead(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return "";
            AddToHead(Tag.Custom(html));
            return "";
        }

        /// <inheritdoc />
        public string AddMeta(string name, string content) => AddToHead(_htmlTagsLazy.Value.Meta().Name(name).Content(content));
    }
}
