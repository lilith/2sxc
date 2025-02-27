﻿using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Lib.Logging;
using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        private List<string> GetMetadataTypeNames(object target, string contentTypes)
        {
            var types = contentTypes?.Split(',').Select(s => s.Trim()).ToArray() ?? Array.Empty<string>();
            if (!types.Any())
                types = FindMetadataRecommendations(target);

            var finalTypes = new List<string>();
            foreach (var type in types)
                if (type == "*") finalTypes.AddRange(FindMetadataRecommendations(target));
                else finalTypes.Add(type);
            return finalTypes;
        }

        private string[] FindMetadataRecommendations(object target) => Log.Func(() =>
        {
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (target == null)
                return (Array.Empty<string>(), "null");

            if (target is IHasMetadata withMetadata)
                target = withMetadata.Metadata;

            if (!(target is IMetadataOf mdOf))
                return (Array.Empty<string>(), "not metadata");

            var recommendations = mdOf?.Target?.Recommendations ?? Array.Empty<string>();

            return (recommendations, "ok");
        });
    }
}
