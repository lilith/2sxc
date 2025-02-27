﻿using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Edit.Toolbar
{
    internal class ToolbarRuleToolbar: ToolbarRule
    {
        internal const string Empty = "empty";
        internal const string Default = "default";

        public ToolbarRuleToolbar(string template = "", string ui = ""): base("toolbar", ui: ui)
        {
            CommandValue = template;
        }

        public bool IsDefault => TemplateName == Default;

        public string TemplateName => CommandValue.HasValue() ? CommandValue : Default;
    }
}
