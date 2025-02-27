﻿using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Data;
using ToSic.Eav.ImportExport.Json;
using ToSic.Eav.ImportExport.Json.V1;
using ToSic.Lib.Logging;
using ToSic.Eav.WebApi.Formats;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class EditLoadBackend
    {

        /// <summary>
        /// Get Serialized entity or create a new one, and assign metadata
        /// based on the header (if none already existed)
        /// </summary>
        /// <returns></returns>
        private JsonEntity GetSerializeAndMdAssignJsonEntity(int appId, BundleWithHeader<IEntity> bundle, JsonSerializer jsonSerializer,
            ContentTypeRuntime typeRead, AppState appState) => Log.Func(l =>
        {
            // attach original metadata assignment when creating a new one
            JsonEntity ent;
            if (bundle.Entity != null)
            {
                ent = jsonSerializer.ToJson(bundle.Entity, 1);

            }
            else
            {
                ent = jsonSerializer.ToJson(ConstructEmptyEntity(appId, bundle.Header, typeRead), 0);

                // only attach metadata, if no metadata already exists
                if (ent.For == null && bundle.Header?.For != null) ent.For = bundle.Header.For;
            }

            // new UI doesn't use this any more, reset it
            if (bundle.Header != null) bundle.Header.For = null;

            try
            {
                if (ent.For != null)
                {
                    var targetId = ent.For;
                    // #TargetTypeIdInsteadOfTarget
                    var targetType = targetId.TargetType != 0
                        ? targetId.TargetType
                        : jsonSerializer.MetadataTargets.GetId(targetId.Target);
                    ent.For.Title = appState.FindTargetTitle(targetType,
                        targetId.String ?? targetId.Guid?.ToString() ?? targetId.Number?.ToString());
                }
            }
            catch { /* ignore experimental */ }

            return (ent);
        });

        internal static List<IContentType> UsedTypes(List<BundleWithHeader<IEntity>> list, ContentTypeRuntime typeRead)
            => list.Select(i
                    // try to get the entity type, but if there is none (new), look it up according to the header
                    => i.Entity?.Type
                       ?? typeRead.Get(i.Header.ContentTypeName))
                .ToList();

        internal List<InputTypeInfo> GetNecessaryInputTypes(List<JsonContentType> contentTypes, ContentTypeRuntime typeRead)
        {
            var wrapLog = Log.Fn<List<InputTypeInfo>>($"{nameof(contentTypes)}: {contentTypes.Count}");
            var fields = contentTypes
                .SelectMany(t => t.Attributes)
                .Select(a => a.InputType)
                .Distinct()
                .ToList();

            Log.A("Found these input types to load: " + string.Join(", ", fields));

            var allInputType = typeRead.GetInputTypes();

            var found = allInputType
                .Where(it => fields.Contains(it.Type))
                .ToList();

            if (found.Count == fields.Count) Log.A("Found all");
            else
            {
                Log.A(
                    $"It seems some input types were not found. Needed {fields.Count}, found {found.Count}. Will try to log details for this.");
                try
                {
                    var notFound = fields.Where(field => found.All(fnd => fnd.Type != field));
                    Log.A("Didn't find: " + string.Join(",", notFound));
                }
                catch (Exception)
                {
                    Log.A("Ran into problems logging missing input types.");
                }
            }

            return wrapLog.Return(found, $"{found.Count}");
        }

        private IEntity ConstructEmptyEntity(int appId, ItemIdentifier header, ContentTypeRuntime typeRead) => Log.Func(() =>
        {
            var type = typeRead.Get(header.ContentTypeName);
            var ent = _entityBuilder.EmptyOfType(appId, header.Guid, header.EntityId, type);
            return ent;
        });
    }
}
