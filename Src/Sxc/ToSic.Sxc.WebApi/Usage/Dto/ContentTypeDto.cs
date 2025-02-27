﻿using ToSic.Eav.Data;
using ToSic.Eav.WebApi.Dto;

namespace ToSic.Sxc.WebApi.Usage.Dto
{
    public class ContentTypeDto: IdentifierDto
    {
        public string Name;
        public string StaticName;

        public ContentTypeDto(IContentType type)
        {
            Id = type.Id;
            Name = type.Name;
            StaticName = type.NameId;
        }
    }
}
