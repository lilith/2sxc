﻿using ToSic.Sxc.Apps.Paths;

namespace ToSic.Sxc.WebApi.Admin.AppFiles
{
    public partial class AppFilesControllerReal
    {
        private string ResolveAppPath(int appId, bool global) =>
            (_appPaths.InitDone
                ? _appPaths
                : _appPaths.Init(_site, _appStates.Get(appId)))
            .PhysicalPathSwitch(global);
    }
}
