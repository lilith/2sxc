﻿using System.Web.Http.Routing;
using ToSic.Eav.WebApi.Routing;

namespace ToSic.Sxc.Dnn.WebApiRouting
{
    internal class Route
    {
        public static string AppPathOrNull(IHttpRouteData route) => route.Values[VarNames.AppPath]?.ToString();
    }
}
