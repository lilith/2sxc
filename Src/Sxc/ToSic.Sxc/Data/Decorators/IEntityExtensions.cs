﻿using ToSic.Eav.Data;

namespace ToSic.Sxc.Data.Decorators
{
    public static class IEntityExtensions
    {
        public static bool IsDemoItemSafe(this IEntity entity) => entity?.GetDecorator<EntityInBlockDecorator>()?.IsDemoItem ?? false;

        public static bool DisableInlineEditSafe(this IEntity entity)
        {
            if (entity == null) return true;
            return entity.GetDecorator<CmsEditDecorator>()?.DisableEdit ?? IsDemoItemSafe(entity);
        }
    }
}