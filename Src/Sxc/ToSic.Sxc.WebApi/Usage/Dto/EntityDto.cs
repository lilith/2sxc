﻿using ToSic.Eav.Data;
using ToSic.Eav.WebApi.Dto;

namespace ToSic.Sxc.WebApi.Usage.Dto
{
    public class EntityDto: IdentifierDto
    {
        public string Title;
        public ContentTypeDto Type;

        public EntityDto(IEntity entity)
        {
            Id = entity.EntityId;
            Guid = entity.EntityGuid;
            Title = entity.GetBestTitle();
            Type = new ContentTypeDto(entity.Type);
        }
    }
}
