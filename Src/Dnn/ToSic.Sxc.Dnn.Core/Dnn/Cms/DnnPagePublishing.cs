﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Decorators;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sxc.Dnn.Run;

using IEntity = ToSic.Eav.Data.IEntity;
using ServiceBase = ToSic.Lib.Services.ServiceBase;

namespace ToSic.Sxc.Dnn.Cms
{
    public partial class DnnPagePublishing : ServiceBase, IPagePublishing
    {
        #region DI Constructors and More

        private readonly LazySvc<AppManager> _appManager;
        private readonly LazySvc<IModuleAndBlockBuilder> _moduleAndBlockBuilder;

        public DnnPagePublishing(LazySvc<AppManager> appManager, LazySvc<IModuleAndBlockBuilder> moduleAndBlockBuilder) : base("Dnn.Publsh")
        {
            ConnectServices(
                _appManager = appManager,
                _moduleAndBlockBuilder = moduleAndBlockBuilder
            );
        }
        
        #endregion

        public void DoInsidePublishing(IContextOfSite context, Action<VersioningActionInfo> action)
        {
            var possibleContextOfBlock = context as IContextOfBlock;
            var enabled = possibleContextOfBlock?.Publishing.ForceDraft ?? false;
            var instanceId = possibleContextOfBlock?.Module.Id ?? Eav.Constants.IdNotInitialized;
            var userId = (context.User as DnnUser)?.UnwrappedContents.UserID ?? Eav.Constants.IdNotInitialized;
            Log.A($"DoInsidePublishing(module:{instanceId}, user:{userId}, enabled:{enabled})");

            if (enabled)
            {
                var moduleVersionSettings = new ModuleVersions(instanceId, Log);
                
                // Get an new version number and submit it to DNN
                // The submission must be made every time something changes, because a "discard" could have happened
                // in the meantime.
                TabChangeTracker.Instance.TrackModuleModification(
                    moduleVersionSettings.ModuleInfo, 
                    moduleVersionSettings.IncreaseLatestVersion(), 
                    userId
                );
            }

            var versioningActionInfo = new VersioningActionInfo();
            action.Invoke(versioningActionInfo);
            Log.A("/DoInsidePublishing");
        }



        public int GetLatestVersion(int instanceId)
        {
            var moduleVersionSettings = new ModuleVersions(instanceId, Log);
            var ver = moduleVersionSettings.GetLatestVersion();
            Log.A($"GetLatestVersion(m:{instanceId}) = ver:{ver}");
            return ver;
        }

        public int GetPublishedVersion(int instanceId)
        {
            var moduleVersionSettings = new ModuleVersions(instanceId, Log);
            var pubVersion = moduleVersionSettings.GetPublishedVersion();
            Log.A($"GetPublishedVersion(m:{instanceId}) = pub:{pubVersion}");
            return pubVersion;
        }


        public void Publish(int instanceId, int version)
        {
            Log.A($"Publish(m:{instanceId}, v:{version})");
            try
            {
                // publish all entities of this content block
                var dnnModule = ModuleController.Instance.GetModule(instanceId, Null.NullInteger, true);
                // must find tenant through module, as the Portal-Settings.Current is null in search mode
                var cb = _moduleAndBlockBuilder.Value.GetProvider(dnnModule, null).LoadBlock();

                Log.A($"found dnn mod {cb.Context.Module.Id}, tenant {cb.Context.Site.Id}, cb exists: {cb.ContentGroupExists}");
                if (cb.ContentGroupExists)
                {
                    Log.A("cb exists");
                    var appManager = _appManager.Value.Init(cb);

                    // Add content entities
                    IEnumerable<IEntity> list = new List<IEntity>();
                    list = TryToAddStream(list, cb.Data, DataSourceConstants.StreamDefaultName);
                    list = TryToAddStream(list, cb.Data, "ListContent");
                    list = TryToAddStream(list, cb.Data, "PartOfPage");

                    // ReSharper disable PossibleMultipleEnumeration
                    // Find related presentation entities
                    var attachedPresItems = list
                        .Select(e => e.GetDecorator<EntityInBlockDecorator>()?.Presentation)
                        .Where(p => p != null);
                    Log.A($"adding presentation item⋮{attachedPresItems.Count()}");
                    list = list.Concat(attachedPresItems);
                    // ReSharper restore PossibleMultipleEnumeration

                    var ids = list.Where(e => !e.IsPublished).Select(e => e.EntityId).ToList();

                    // publish BlockConfiguration as well - if there already is one
                    if (cb.Configuration != null)
                    {
                        Log.A($"add group id:{cb.Configuration.Id}");
                        ids.Add(cb.Configuration.Id);
                    }

                    Log.A(Log.Try(() => $"will publish id⋮{ids.Count} ids:[{ string.Join(",", ids.Select(i => i.ToString()).ToArray()) }]"));

                    if (ids.Any())
                        appManager.Entities.Publish(ids.ToArray());
                    else
                        Log.A("no ids found, won\'t publish items");
                }

                // Set published version
                new ModuleVersions(instanceId, Log).PublishLatestVersion();
                Log.A("publish completed");
            }
            catch (Exception ex)
            {
                DnnLogging.LogToDnn("exception", "publishing", Log, force: true);
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                throw;
            }

        }

        private IEnumerable<IEntity> TryToAddStream(IEnumerable<IEntity> list, IContextData data, string key)
        {
            var cont = data.GetStream(key, nullIfNotFound: true)?.List.ToImmutableList(); //  data.Out.ContainsKey(key) ? data[key]?.List?.ToImmutableList() : null;
            Log.A($"TryToAddStream(..., ..., key:{key}), found:{cont != null} add⋮{cont?.Count ?? 0}" );
            if (cont != null) list = list.Concat(cont);
            return list;
        }

    }
}
