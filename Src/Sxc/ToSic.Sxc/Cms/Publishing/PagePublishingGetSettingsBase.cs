﻿using System.Collections.Concurrent;
using ToSic.Eav.Apps.Enums;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Cms.Publishing
{
    public abstract class PagePublishingGetSettingsBase: ServiceBase, IPagePublishingGetSettings
    {
        protected PagePublishingGetSettingsBase(string logPrefix) : base(logPrefix + ".PubRes") { }

        private PublishingMode Requirements(int instanceId) => Log.Func($"{instanceId}", () =>
        {
            if (instanceId < 0) return (PublishingMode.DraftOptional, "no instance");
            if (Cache.ContainsKey(instanceId)) return (Cache[instanceId], "in cache");

            var decision = LookupRequirements(instanceId);
            Cache.TryAdd(instanceId, decision);
            return (decision, $"decision:{decision}");
        });
        protected static readonly ConcurrentDictionary<int, PublishingMode> Cache = new ConcurrentDictionary<int, PublishingMode>();

        /// <summary>
        /// The lookup must be implemented for each platform
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        protected abstract PublishingMode LookupRequirements(int moduleId);

        public BlockPublishingSettings SettingsOfModule(int moduleId)
        {
            var mode = Requirements(moduleId);
            return new BlockPublishingSettings
            {
                AllowDraft = mode != PublishingMode.DraftForbidden,
                ForceDraft = mode == PublishingMode.DraftRequired, 
                Mode = mode
            };
        }

        #region SwitchableService


        public virtual string NameId => "Default";

        public virtual bool IsViable() => true;

        public virtual int Priority => (int)PagePublishingPriorities.Default;

        #endregion
    }
}
