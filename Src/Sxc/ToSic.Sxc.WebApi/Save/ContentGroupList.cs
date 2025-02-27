﻿using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Data;
using ToSic.Lib.Logging;
using ToSic.Eav.WebApi.Formats;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Edit;
using static System.StringComparison;

namespace ToSic.Sxc.WebApi.Save
{
    public class ContentGroupList: ServiceBase
    {

        #region Constructor / DI

        private readonly LazySvc<BlockEditorSelector> _blockEditorSelectorLazy;
        private readonly LazySvc<CmsManager> _cmsManagerLazy;
        private CmsManager CmsManager => _cmsManager ?? (_cmsManager = _cmsManagerLazy.Value.InitQ(_appIdentity/*, _withDrafts*/));
        private CmsManager _cmsManager;
        //private bool? _withDrafts = false;

        public ContentGroupList(LazySvc<CmsManager> cmsManagerLazy, LazySvc<BlockEditorSelector> blockEditorSelectorLazy) : base("Api.GrpPrc")
        {
            ConnectServices(
                _blockEditorSelectorLazy = blockEditorSelectorLazy,
                _cmsManagerLazy = cmsManagerLazy
            );
        }

        public ContentGroupList Init(IAppIdentity appIdentity/*, bool? withDraftsTemp*/)
        {
            _appIdentity = appIdentity;
            //_withDrafts = withDraftsTemp;
            return this;
        }
        private IAppIdentity _appIdentity;
        #endregion

        internal bool IfChangesAffectListUpdateIt(IBlock block, List<BundleWithHeader<IEntity>> items,
            Dictionary<Guid, int> ids) => Log.Func(() =>
        {
            var groupItems = items
                .Where(i => i.Header.Parent != null)
                .GroupBy(i => i.Header.Parent.Value.ToString() + i.Header.IndexSafeOrFallback() + i.Header.AddSafe)
                .ToList();

            // if it's new, it has to be added to a group
            // only add if the header wants it, AND we started with ID unknown
            return groupItems.Any()
                ? (PostSaveUpdateIdsInParent(block, ids, groupItems), "post save ids")
                : (true, "no additional group processing necessary");
        });

        private bool PostSaveUpdateIdsInParent(IBlock block,
            Dictionary<Guid, int> postSaveIds,
            IEnumerable<IGrouping<string, BundleWithHeader<IEntity>>> pairsOrSingleItems)
        {
            var wrapLog = Log.Fn<bool>($"{_appIdentity.AppId}");

            // If no content block given, skip all this
            if (block == null) return wrapLog.ReturnTrue("no block, nothing to update");


            foreach (var bundle in pairsOrSingleItems)
            {
                Log.A("processing:" + bundle.Key);

                if (bundle.First().Header.Parent == null) continue;

                var parent = CmsManager.Read.AppState.GetDraftOrPublished(bundle.First().Header.GetParentEntityOrError());
                var targetIsContentBlock = parent.Type.Name == BlocksRuntime.BlockTypeName;
                
                var primaryItem = targetIsContentBlock ? FindContentItem(bundle) : bundle.First();
                var primaryId = GetIdFromGuidOrError(postSaveIds, primaryItem.Entity.EntityGuid);

                var ids = targetIsContentBlock
                    ? new[] {primaryId, FindPresentationItem(postSaveIds, bundle)}
                    : new[] {primaryId as int?};

                var index = primaryItem.Header.IndexSafeOrFallback();
                // fix https://github.com/2sic/2sxc/issues/2846 - Bug: Adding an item to a list doesn't seem to respect the position
                // This is used on new content item (+)
                var indexNullAddToEnd = primaryItem.Header.Index == null;
                var willAdd = primaryItem.Header.AddSafe;

                Log.A($"will add: {willAdd}; Group.Add:{primaryItem.Header.Add}; EntityId:{primaryItem.Entity.EntityId}");

                var fieldPair = targetIsContentBlock
                    ? ViewParts.PickFieldPair(primaryItem.Header.Field)
                    : new[] {primaryItem.Header.Field};

                if (willAdd) // this cannot be auto-detected, it must be specified
                {

                    // handle edge case on app with empty list, when index=1, but it should be index=0 (indexNullAddToEnd=true have the same effect)
                    // fix https://github.com/2sic/2sxc/issues/2943 
                    if (!parent.Children(fieldPair.First()).Any() && !targetIsContentBlock) indexNullAddToEnd = true;
                    
                    CmsManager.Entities.FieldListAdd(parent, fieldPair, index, ids, block.Context.Publishing.ForceDraft, indexNullAddToEnd, targetIsContentBlock);
                }
                else
                    CmsManager.Entities.FieldListReplaceIfModified(parent, fieldPair, index, ids, block.Context.Publishing.ForceDraft);

            }

            // update-module-title
            _blockEditorSelectorLazy.Value.GetEditor(block).UpdateTitle();
            return wrapLog.ReturnTrue("ok");
        }

        private static BundleWithHeader<T> FindContentItem<T>(IGrouping<string, BundleWithHeader<T>> bundle)
        {
            var primaryItem = bundle
                                  .FirstOrDefault(e =>
                                      string.Equals(e.Header.Field, ViewParts.Content, OrdinalIgnoreCase))
                              ?? bundle.FirstOrDefault(e =>
                                  string.Equals(e.Header.Field, ViewParts.FieldHeader, OrdinalIgnoreCase));
            if (primaryItem == null)
                throw new Exception("unexpected group-entity assignment, cannot figure it out");
            return primaryItem;
        }

        /// <summary>
        /// Get saved entity (to get its ID)
        /// </summary>
        private static int GetIdFromGuidOrError(IReadOnlyDictionary<Guid, int> postSaveIds, Guid guid)
        {
            if (!postSaveIds.ContainsKey(guid))
                throw new Exception("Saved entity not found - not able to update BlockConfiguration");

            return postSaveIds[guid];
        }

        private int? FindPresentationItem(IReadOnlyDictionary<Guid, int> postSaveIds,
            IGrouping<string, BundleWithHeader<IEntity>> bundle) => Log.Func(() =>
        {
            int? presentationId = null;
            var presItem =
                bundle.FirstOrDefault(e => string.Equals(e.Header.Field, ViewParts.Presentation, OrdinalIgnoreCase))
                ?? bundle.FirstOrDefault(e =>
                    string.Equals(e.Header.Field, ViewParts.ListPresentation, OrdinalIgnoreCase));

            if (presItem == null) return (null, "no presentation");
            // use null if it shouldn't have one
            if (presItem.Header.IsEmpty) return (null, "header is empty");

            if (postSaveIds.ContainsKey(presItem.Entity.EntityGuid))
                presentationId = postSaveIds[presItem.Entity.EntityGuid];

            return (presentationId, "found");
        });

        internal ContentGroupList ConvertGroup(List<ItemIdentifier> identifiers) => Log.Func(() =>
        {
            foreach (var identifier in identifiers.Where(identifier => identifier != null))
                identifier.IsContentBlockMode = DetectContentBlockMode(identifier);
            return this;
        });

        internal List<ItemIdentifier> ConvertListIndexToId(List<ItemIdentifier> identifiers) => Log.Func(() =>
        {
            var newItems = new List<ItemIdentifier>();
            foreach (var identifier in identifiers)
            {
                // Case one, it's a Content-Group - in this case the content-type name comes from View configuration
                if (identifier.IsContentBlockMode)
                {
                    if (!identifier.Parent.HasValue) continue;

                    var contentGroup = CmsManager.Read.Blocks.GetBlockConfig(identifier.GetParentEntityOrError());
                    var contentTypeName = (contentGroup.View as View)?.GetTypeStaticName(identifier.Field) ?? "";

                    // if there is no content-type for this, then skip it (don't deliver anything)
                    if (contentTypeName == "")
                        continue;

                    identifier.ContentTypeName = contentTypeName;
                    ConvertListIndexToEntityIds(identifier, contentGroup);
                    newItems.Add(identifier);
                    continue;
                }

                // Case #2 it's an entity inside a field of another entity
                // Added in v11.01
                if (identifier.Parent != null && identifier.Field != null)
                {
                    // look up type
                    var target = CmsManager.Read.AppState.List.One(identifier.Parent.Value);
                    var field = target.Type[identifier.Field];
                    identifier.ContentTypeName = field.EntityFieldItemTypePrimary();
                    newItems.Add(identifier);
                    continue;
                }

                // Default case - just a normal identifier
                newItems.Add(identifier);
            }

            return newItems;
        });


        /// <summary>
        /// Check if the save will affect a ContentBlock.
        /// If it's a simple entity-edit or edit of item inside a normal field list, it returns false
        /// </summary>
        /// <returns></returns>
        private bool DetectContentBlockMode(ItemIdentifier identifier) => Log.Func(() =>
        {
            if (!identifier.Parent.HasValue) return (false, "no parent");

            // get the entity and determine if it's a content-block. If yes, that should affect the differences in load/save
            var entity = CmsManager.Read.AppState.List.One(identifier.Parent.Value);
            return (entity.Type.Name == BlocksRuntime.BlockTypeName, "type name should match");
        });


        private void ConvertListIndexToEntityIds(ItemIdentifier identifier, BlockConfiguration blockConfiguration) =>
            Log.Do(() =>
            {
                var part = blockConfiguration[identifier.Field];
                if (!identifier.AddSafe) // not in add-mode
                {
                    var idx = identifier.IndexSafeOrFallback(part.Count - 1);
                    if (idx >= 0 && part.Count > idx && // has as many items as desired
                        part[idx] != null) // and the slot has something
                        identifier.EntityId = part[idx].EntityId;
                }

                // tell the UI that it should not actually use this data yet, keep it locked
                if (!identifier.Field.ToLowerInvariant().Contains(ViewParts.PresentationLower))
                    return "no presentation";

                // the following steps are only for presentation items
                identifier.IsEmptyAllowed = true;

                if (identifier.EntityId != 0)
                    return "id != 0";

                identifier.IsEmpty = true;

                identifier.DuplicateEntity = identifier.Field.ToLowerInvariant() == ViewParts.PresentationLower
                    ? blockConfiguration.View.PresentationItem?.EntityId
                    : blockConfiguration.View.HeaderPresentationItem?.EntityId;

                return "ok";
            });
    }
}
