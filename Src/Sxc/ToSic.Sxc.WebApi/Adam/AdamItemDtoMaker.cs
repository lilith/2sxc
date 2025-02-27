﻿using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.ImportExport.Json.V1;
using ToSic.Eav.Metadata;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Dto.Metadata;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.WebApi.Adam
{
    public class AdamItemDtoMaker<TFolderId, TFileId>
    {
        #region Constructor / DI

        public class MyServices
        {
            public AdamSecurityChecksBase Security { get; }
            public MyServices(AdamSecurityChecksBase security)
            {
                Security = security;
            }
        }

        public AdamItemDtoMaker(MyServices services)
        {
            _security = services.Security;
        }

        public AdamItemDtoMaker<TFolderId, TFileId> Init(AdamContext adamContext)
        {
            AdamContext = adamContext;
            return this;
        }

        private readonly AdamSecurityChecksBase _security;
        public AdamContext AdamContext;

        #endregion

        private const string ThumbnailPattern = "{0}?w=120&h=120&mode=crop&urlSource=backend";
        private const string PreviewPattern = "{0}?w=800&h=800&mode=max&urlSource=backend";

        public virtual AdamItemDto Create(File<TFolderId, TFileId> file)
        {
            var url = file.Url;
            var item = new AdamItemDto<TFolderId, TFileId>(false, file.SysId, file.ParentSysId,
                file.FullName, file.Size,
                file.Created, file.Modified)
            {
                Path = file.Path,
                ThumbnailUrl = string.Format(ThumbnailPattern, url),
                PreviewUrl = string.Format(PreviewPattern, url),
                Url = url,
                ReferenceId = (file as IHasMetadata).Metadata.Target.KeyString,
                AllowEdit = CanEditFolder(file),
                Metadata = GetMetadataOf(file.Metadata),
                Type = Classification.TypeName(file.Extension),
            };
            return item;
        }


        public virtual AdamItemDto Create(Folder<TFolderId, TFileId> folder)
        {
            var item = new AdamItemDto<TFolderId, TFolderId>(true, folder.SysId, folder.ParentSysId, folder.Name, 0,
                folder.Created,
                folder.Modified)
            {
                Path = folder.Path,
                AllowEdit = CanEditFolder(folder),
                ReferenceId = (folder as IHasMetadata).Metadata.Target.KeyString,
                //MetadataId = (int)folder.Metadata.EntityId,
                Metadata = GetMetadataOf(folder.Metadata),
            };
            return item;
        }

        private IEnumerable<MetadataOfDto> GetMetadataOf(IMetadata md)
        {
            if (md == null) return null;

            var result = ((IHasMetadata)md).Metadata
                .Select(m => new MetadataOfDto
                {
                    Id = m.EntityId,
                    Guid = m.EntityGuid,
                    Type = new JsonType(m)
                })
                .ToArray();
            return result.Any() ? result : null;
        }

        private bool CanEditFolder(Eav.Apps.Assets.IAsset original)
        {
            return AdamContext.UseSiteRoot
                ? _security.CanEditFolder(original)
                : ContextAllowsEdit;
        }

        /// <summary>
        /// Do this check once only, as the result will never change during one lifecycle
        /// </summary>
        private bool ContextAllowsEdit 
            => _contextAllowsEdit 
               ?? (_contextAllowsEdit = !AdamContext.Security.UserIsRestricted || AdamContext.Security.FieldPermissionOk(GrantSets.WriteSomething)).Value;
        private bool? _contextAllowsEdit;


    }
}
