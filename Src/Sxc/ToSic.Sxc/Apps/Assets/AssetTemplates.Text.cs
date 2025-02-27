﻿using ToSic.Sxc.Context;

namespace ToSic.Sxc.Apps.Assets
{
    public partial class AssetTemplates
    {
        private static readonly TemplateInfo EmptyTextFile = new TemplateInfo("txt", "text file", ".txt", "Notes", ForDocs, TypeNone)
        {
            Body = @"Simple text file.
",
            Description = "Simple text file",
        };

        private static readonly TemplateInfo EmptyFile = new TemplateInfo("empty", "Empty file", "", "some-file.txt", ForDocs, TypeNone)
        {
            Body = "",
            Description = "Simple empty file",
            PlatformTypes = null,
        };
    }
}
