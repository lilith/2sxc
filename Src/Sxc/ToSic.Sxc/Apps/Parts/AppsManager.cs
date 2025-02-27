﻿using System;
using System.IO;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Apps.Paths;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Apps
{
    public class AppsManager: ZonePartRuntimeBase 
    {
        #region Constructor / DI

        public AppsManager(LazySvc<ZoneManager> zoneManagerLazy, IAppStates appStates, ISite site, AppPaths appPaths) : base("Cms.AppsRt")
        {
            ConnectServices(
                _zoneManagerLazy = zoneManagerLazy,
                _appStates = appStates,
                _site = site,
                _appPaths = appPaths
            );
        }
        private readonly LazySvc<ZoneManager> _zoneManagerLazy;
        private readonly IAppStates _appStates;
        private readonly ISite _site;
        private readonly AppPaths _appPaths;

        #endregion


        internal void RemoveAppInSiteAndEav(int appId, bool fullDelete)
        {
            var zoneId = ZoneRuntime.ZoneId;
            // check portal assignment and that it's not the default app
            // enable restore for DefaultApp
            if (appId == _appStates.DefaultAppId(zoneId) && fullDelete)
                throw new Exception("The default app of a zone cannot be removed.");

            if (appId == Eav.Constants.MetaDataAppId)
                throw new Exception("The special old global app cannot be removed.");

            // todo: maybe verify the app is of this portal; I assume delete will fail anyhow otherwise

            // Prepare to Delete folder in dnn - this must be done, before deleting the app in the DB
            var appState = _appStates.Get(new AppIdentity(zoneId, appId));
            var paths = _appPaths.Init(_site, appState);
            var folder = appState.Folder;
            var physPath = paths.PhysicalPath;

            // now remove from DB. This sometimes fails, so we do this before trying to clean the files
            // as the db part should be in a transaction, and if it fails, everything should stay as is
            _zoneManagerLazy.Value.SetId(zoneId).DeleteApp(appId, fullDelete);

            // now really delete the files - if the DB didn't end up throwing an error
            // ...but only if it's a full-delete
            if (fullDelete && !string.IsNullOrEmpty(folder) && Directory.Exists(physPath))
                ZipImport.TryToDeleteDirectory(physPath, Log);
        }
    }
}
