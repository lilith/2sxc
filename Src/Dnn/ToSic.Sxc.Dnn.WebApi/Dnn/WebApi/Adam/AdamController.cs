﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.WebApi.Adam;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.WebApi;
using RealController = ToSic.Sxc.WebApi.Adam.AdamControllerReal<int>;

namespace ToSic.Sxc.Dnn.WebApi
{
    /// <summary>
    /// Direct access to app-content items, simple manipulations etc.
    /// Should check for security at each standard call - to see if the current user may do this
    /// Then we can reduce security access level to anonymous, because each method will do the security check
    /// </summary>
    [SupportedModules(DnnSupportedModuleNames)]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]    // use view, all methods must re-check permissions
    [ValidateAntiForgeryToken]
    public class AdamController : SxcApiControllerBase, IAdamController<int>
    {


        public AdamController() : base("Adam") { }
        private RealController Real => SysHlp.GetService<RealController>();

        [HttpPost]
        [HttpPut]
        public object Upload(int appId, string contentType, Guid guid, string field, [FromUri] string subFolder = "", bool usePortalRoot = false) 
            => Real.Upload(new HttpUploadedFile(Request, HttpContext.Current.Request), appId, contentType, guid, field, subFolder, usePortalRoot);

        // Note: #AdamItemDto - as of now, we must use object because System.Io.Text.Json will otherwise not convert the object correctly :(

        [HttpGet]
        public IEnumerable</*AdamItemDto*/object> Items(int appId, string contentType, Guid guid, string field, string subfolder, bool usePortalRoot = false)
            => Real.Items(appId, contentType, guid, field, subfolder, usePortalRoot);


        [HttpPost]
        public IEnumerable</*AdamItemDto*/object> Folder(int appId, string contentType, Guid guid, string field, string subfolder, string newFolder, bool usePortalRoot)
            => Real.Folder(appId, contentType, guid, field, subfolder, newFolder, usePortalRoot);


        [HttpGet]
        public bool Delete(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, int id, bool usePortalRoot)
            => Real.Delete(appId, contentType, guid, field, subfolder, isFolder, id, usePortalRoot);


        [HttpGet]
        public bool Rename(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, int id, string newName, bool usePortalRoot)
            => Real.Rename(appId, contentType, guid, field, subfolder, isFolder, id, newName, usePortalRoot);

    }
}