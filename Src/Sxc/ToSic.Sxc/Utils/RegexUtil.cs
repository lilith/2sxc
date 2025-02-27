﻿using System;
using System.Text.RegularExpressions;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Web.PageService;

namespace ToSic.Sxc.Utils
{
    public class RegexUtil
    {
        /// <summary>
        /// Used in ScriptSrc and StyleSrc Formulas
        /// </summary>
        public const string SrcKey = "Src";

        public const string IntegrityKey = "Integrity";

        // language=regex
        private const string AttributesFormula = "\\s(?<Key>[\\w-]+(?=[^<]*>))=([\"'])(?<Value>.*?[^\\1][\\s\\S]+?)\\1|\\s(?<Key>[\\w-]+(?=.*?))";
        // language=regex
        private const string ImagesWithDataCmsidFormula = "<img[^>]*data-cmsid=['\"](?<cmsid>[^'\"]+)['\"][^>]*>";
        // language=regex
        private const string ScriptSrcFormula = "<script\\s([^>]*)src=('|\")(?<Src>.*?)('|\")(([^>]*/>)|[^>]*(>.*?</script>))";
        // language=regex
        private const string IntegrityAttributeFormula = "(?<Integrity>integrity=('|\").*?('|\"))";

        // language=regex
        private const string ScriptContentFormula = @"<script[^>]*>(?<Content>(.|\n)*?)</script[^>]*>";
        // language=regex
        private const string StyleSrcFormula = "<link\\s([^>]*)href=('|\")(?<Src>.*?)('|\")([^>]*)(>.*?</link>|/?>)";
        // language=regex
        private const string StyleRelFormula = "('|\"|\\s)rel=('|\")stylesheet('|\")";
        // language=regex
        private const string IdFormula = "('|\"|\\s)id=('|\")(?<Id>.*?)('|\")";
        // language=regex
        private const string ClientDependencyRegex =
            "\\s" + PageService.AssetOptimizationsAttributeName + "=('|\")(?<Priority>true|[0-9]+)?(?::)?(?<Position>bottom|head|body)?('|\")(>|\\s)";

        public const string PriorityKey = "Priority";
        public const string PositionKey = "Position";

        public static readonly Lazy<Regex> AttributesDetection = new Lazy<Regex>(() => new Regex(AttributesFormula, RegexOptions.IgnoreCase));
        public static readonly Lazy<Regex> ImagesDetection = new Lazy<Regex>(() => new Regex(ImagesWithDataCmsidFormula, RegexOptions.IgnoreCase));
        public static readonly Lazy<Regex> ScriptSrcDetection = new Lazy<Regex>(() => new Regex(ScriptSrcFormula, RegexOptions.IgnoreCase | RegexOptions.Singleline));
        // note: 2dm created this, because I wasn't sure if changing the original to ML would have side effects
        public static Regex ScriptSrcDetectionMultiLine => ScriptSrcDetMl.Get(() => new Regex(ScriptSrcFormula, RegexOptions.IgnoreCase | RegexOptions.Multiline));
        private static readonly GetOnce<Regex> ScriptSrcDetMl = new GetOnce<Regex>();

        public static readonly Lazy<Regex> ScriptContentDetection = new Lazy<Regex>(() => new Regex(ScriptContentFormula, RegexOptions.IgnoreCase | RegexOptions.Multiline));
        public static readonly Lazy<Regex> StyleDetection = new Lazy<Regex>(() => new Regex(StyleSrcFormula, RegexOptions.IgnoreCase | RegexOptions.Singleline));
        // note: 2dm created this, because I wasn't sure if changing the original to ML would have side effects
        public static Regex StyleDetectionMultiLine => StyleSrcDetMl.Get(() => new Regex(StyleSrcFormula, RegexOptions.IgnoreCase | RegexOptions.Multiline));
        private static readonly GetOnce<Regex> StyleSrcDetMl = new GetOnce<Regex>();
        public static Regex IntegrityAttribute => IntegrAttr.Get(() => new Regex(IntegrityAttributeFormula, RegexOptions.IgnoreCase | RegexOptions.Singleline));
        private static readonly GetOnce<Regex> IntegrAttr = new GetOnce<Regex>();
        public static readonly Lazy<Regex> StyleRelDetect = new Lazy<Regex>(() => new Regex(StyleRelFormula, RegexOptions.IgnoreCase));
        public static readonly Lazy<Regex> OptimizeDetection = new Lazy<Regex>(() => new Regex(ClientDependencyRegex, RegexOptions.IgnoreCase));
        public static readonly Lazy<Regex> IdDetection = new Lazy<Regex>(() => new Regex(IdFormula, RegexOptions.IgnoreCase));

        //// language=regex
        //private const string WysiwygWidthNumFormula = "wysiwyg-width(?<num>\\d+)of(?<all>\\d+)";
        //private static readonly Lazy<Regex> WysiwygWidthNumDetectionLazy = new Lazy<Regex>(() => new Regex(WysiwygWidthNumFormula, RegexOptions.IgnoreCase));
        //public static Regex WysiwygWidthNumDetection => WysiwygWidthNumDetectionLazy.Value;

        // language=regex
        private const string WysiwygWidthFormula = "wysiwyg-(?<percent>\\d+)";
        public static readonly Lazy<Regex> WysiwygWidthLazy = new Lazy<Regex>(() => new Regex(WysiwygWidthFormula, RegexOptions.IgnoreCase));

    }
}