﻿using ToSic.Eav.Run;
using ToSic.Eav.Run.Unknown;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web.JsContext;

namespace ToSic.Sxc.Services
{
    public class JsApiServiceUnknown : ServiceBase, IJsApiService, IIsUnknown
    {
        public JsApiServiceUnknown(WarnUseOfUnknown<BasicEnvironmentInstaller> _) : base($"{LogScopes.NotImplemented}.JsApi")
        { }
        public string GetJsApiJson(int? pageId, string siteRoot, string rvt) => null;
        public JsApi GetJsApi(int? pageId, string siteRoot, string rvt) => null;
    }
}