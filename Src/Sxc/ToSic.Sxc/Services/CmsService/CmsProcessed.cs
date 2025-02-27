﻿namespace ToSic.Sxc.Services.CmsService
{
    internal class CmsProcessed
    {
        public bool IsProcessed { get; set; }

        public string Contents { get; set; }
        public string Classes { get; set; }

        public string DefaultTag { get; set; } = "div";

        public CmsProcessed(bool isProcessed, string contents, string classes)
        {
            IsProcessed = isProcessed;
            Contents = contents;
            Classes = classes;
        }
    }
}
