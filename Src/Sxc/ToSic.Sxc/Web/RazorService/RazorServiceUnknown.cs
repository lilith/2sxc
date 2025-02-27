﻿using ToSic.Eav.Run.Unknown;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Web
{
    public class RazorServiceUnknown : IRazorService
    {
        public RazorServiceUnknown(WarnUseOfUnknown<RazorServiceUnknown> _) { }

        public string Render(string partialName, object model) => "";

        public void ConnectToRoot(IDynamicCodeRoot codeRoot) { /* Do nothing */ }
    }
}