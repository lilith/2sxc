﻿using System;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Services
{
    internal class FeaturesService: ServiceBase, IFeaturesService, ICanDebug
    {
        public FeaturesService(Eav.Configuration.IFeaturesInternal root) : base($"{Constants.SxcLogName}.FeatSv")
            => _root = root;

        private readonly Eav.Configuration.IFeaturesInternal _root;

        public bool IsEnabled(params string[] nameIds)
        {
            var result = _root.IsEnabled(nameIds);
            if (!Debug) return result;
            var wrapLog = Log.Fn<bool>(string.Join(",", nameIds ?? Array.Empty<string>()));
            return wrapLog.Return(result, $"{result}");
        }

        public bool Debug { get; set; }
    }
}
