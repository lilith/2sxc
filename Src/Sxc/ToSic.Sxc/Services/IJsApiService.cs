﻿using ToSic.Sxc.Web.JsContext;

namespace ToSic.Sxc.Services
{
    public interface IJsApiService
    {
        string GetJsApiJson(int? pageId = null, string siteRoot = null, string rvt = null);
        JsApi GetJsApi(int? pageId = null, string siteRoot = null, string rvt = null);
    }
}
