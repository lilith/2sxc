﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource.Query;
using ToSic.Eav.ImportExport;
using ToSic.Eav.ImportExport.Json;
using ToSic.Eav.ImportExport.Json.V1;
using ToSic.Eav.ImportExport.Serialization;
using ToSic.Eav.ImportExport.Validation;
using ToSic.Lib.Logging;
using ToSic.Eav.Persistence.Logging;
using ToSic.Eav.Run;
using ToSic.Eav.WebApi.Assets;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Validation;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Paths;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.WebApi.ImportExport;
using ToSic.Eav.Helpers;
using ToSic.Eav.Security;
using ToSic.Eav.WebApi.Infrastructure;
#if NETFRAMEWORK
using THttpResponseType = System.Net.Http.HttpResponseMessage;
#else
using THttpResponseType = Microsoft.AspNetCore.Mvc.IActionResult;
#endif

namespace ToSic.Sxc.WebApi.Views
{
    public class ViewsExportImport : ServiceBase
    {
        private readonly LazySvc<QueryDefinitionBuilder> _qDefBuilder;
        private readonly IServerPaths _serverPaths;
        private readonly IEnvironmentLogger _envLogger;
        private readonly LazySvc<CmsManager> _cmsManagerLazy;
        private readonly LazySvc<JsonSerializer> _jsonSerializerLazy;
        private readonly IAppStates _appStates;
        private readonly AppIconHelpers _appIconHelpers;
        private readonly Generator<ImpExpHelpers> _impExpHelpers;
        private readonly IResponseMaker _responseMaker;
        private readonly ISite _site;
        private readonly IUser _user;

        public ViewsExportImport(IServerPaths serverPaths,
            IEnvironmentLogger envLogger,
            LazySvc<CmsManager> cmsManagerLazy, 
            LazySvc<JsonSerializer> jsonSerializerLazy, 
            IContextOfSite context,
            IAppStates appStates,
            AppIconHelpers appIconHelpers,
            Generator<ImpExpHelpers> impExpHelpers,
            IResponseMaker responseMaker,
            LazySvc<QueryDefinitionBuilder> qDefBuilder) : base("Bck.Views")
        {
            ConnectServices(
                _serverPaths = serverPaths,
                _envLogger = envLogger,
                _cmsManagerLazy = cmsManagerLazy,
                _jsonSerializerLazy = jsonSerializerLazy,
                _appStates = appStates,
                _appIconHelpers = appIconHelpers,
                _impExpHelpers = impExpHelpers,
                _responseMaker = responseMaker,
                _qDefBuilder = qDefBuilder
            );
            _site = context.Site;
            _user = context.User;
        }

        public THttpResponseType DownloadViewAsJson(int appId, int viewId)
        {
            var logCall = Log.Fn<THttpResponseType>($"{appId}, {viewId}");
            SecurityHelpers.ThrowIfNotSiteAdmin(_user, Log);
            var app = _impExpHelpers.New().GetAppAndCheckZoneSwitchPermissions(_site.ZoneId, appId, _user, _site.ZoneId);
            var cms = _cmsManagerLazy.Value.Init(app);
            var bundle = new BundleEntityWithAssets
            {
                Entity = app.Data[Eav.ImportExport.Settings.TemplateContentType].One(viewId)
            };

            // Attach files
            var view = new View(bundle.Entity, new[] { _site.CurrentCultureCode }, Log, _qDefBuilder);

            if (!string.IsNullOrEmpty(view.Path))
            {
                TryAddAsset(bundle, app.ViewPath(view, PathTypes.PhysRelative), view.Path);
                var webPath = _appIconHelpers.IconPathOrNull(app, view, PathTypes.PhysRelative)?.ForwardSlash();
                if(webPath != null)
                {
                    var relativePath = webPath.Replace(app.RelativePath.ForwardSlash(), "").TrimPrefixSlash();
                    TryAddAsset(bundle, webPath, relativePath);
                }
            }

            var serializer = _jsonSerializerLazy.Value.SetApp(cms.AppState);
            var serialized = serializer.Serialize(bundle, 0);

            return logCall.ReturnAsOk(_responseMaker.File(serialized,
                ("View" + "." + bundle.Entity.GetBestTitle() + ImpExpConstants.Extension(ImpExpConstants.Files.json))
                .RemoveNonFilenameCharacters()));
        }

        private void TryAddAsset(BundleEntityWithAssets bundle, string webPath, string relativePath)
        {
            if (string.IsNullOrEmpty(webPath)) return;
            var realPath = _serverPaths.FullAppPath(webPath);
            var jsonAssetMan = new JsonAssets();
            var asset1 = jsonAssetMan.Get(realPath, relativePath);
            bundle.Assets.Add(asset1);
        }

        

        public ImportResultDto ImportView(int zoneId, int appId, List<FileUploadDto> files, string defaultLanguage)
        {
            var callLog = Log.Fn<ImportResultDto>($"{zoneId}, {appId}, {defaultLanguage}");

            try
            {
                // 0.1 Check permissions, get the app, 
                var app = _impExpHelpers.New().GetAppAndCheckZoneSwitchPermissions(_site.ZoneId, appId, _user, _site.ZoneId);

                // 0.2 Verify it's json etc.
                if (files.Any(file => !Json.IsValidJson(file.Contents)))
                    throw new ArgumentException("a file is not json");

                // 1. create the views
                var serializer = _jsonSerializerLazy.Value.SetApp(_appStates.Get(app));

                var bundles = files.Select(f => serializer.DeserializeEntityWithAssets(f.Contents)).ToList();

                if (bundles.Any(t => t == null))
                    throw new NullReferenceException("At least one file returned a null-item, something is wrong");

                // 1.1 Verify these are view-entities
                if (!bundles.All(v => v.Entity.Type.Is(Eav.ImportExport.Settings.TemplateContentType)))
                    throw new Exception("At least one of the uploaded items is not a view configuration. " +
                                        "Expected all to be " + Eav.ImportExport.Settings.TemplateContentType);

                // 2. Import the views
                // todo: construction of this should go into init
                _cmsManagerLazy.Value.Init(app.AppId).Entities.Import(bundles.Select(v => v.Entity).ToList());

                // 3. Import the attachments
                var assets = bundles.SelectMany(b => b.Assets);
                var assetMan = new JsonAssets();
                foreach (var asset in assets) assetMan.Create(GetRealPath(app, asset), asset);

                // 3. possibly show messages / issues
                return callLog.ReturnAsOk(new ImportResultDto(true));
            }
            catch (Exception ex)
            {
                _envLogger.LogException(ex);
                return callLog.Return(new ImportResultDto(false, ex.Message, Message.MessageTypes.Error), "error");
            }
        }

        private string GetRealPath(Apps.IApp app, JsonAsset asset)
        {
            if (!string.IsNullOrEmpty(asset.Storage) && asset.Storage != JsonAsset.StorageApp) return null;
            var root = app.PhysicalPathSwitch(false);
            return Path.Combine(root, asset.Folder, asset.Name);
        }
    }
}
