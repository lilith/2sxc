﻿using Microsoft.AspNetCore.Mvc;
using ToSic.Sxc.Oqt.Server.Adam;

namespace ToSic.Sxc.Oqt.Server.WebApi.App
{
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)] // use view, all methods must re-check permissions
    //[Authorize(Roles = RoleNames.Everyone)] commented because of http403 issue
    // TODO: 2DM please check permissions

    // Release routes
    [Route(OqtWebApiConstants.AppRootNoLanguage + "/{appName}/assets")]
    [Route(OqtWebApiConstants.AppRootPathOrLang + "/{appName}/assets")]
    [Route(OqtWebApiConstants.AppRootPathNdLang + "/{appName}/assets")]

    // Beta routes
    //[Route(WebApiConstants.WebApiStateRoot + "/assets/{appName}")]
    public class AppAssetsController: AppAssetsControllerBase
    {
        public AppAssetsController(MyServices services) 
            : base(services, OqtAssetsFileHelper.RouteAssets, "Assets") { }
    }
}
