﻿using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Data.Decorators;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// This is a helper in charge of the list-behavior of a DynamicEntity
    /// </summary>
    [PrivateApi]
    internal class DynamicEntityListHelper
    {
        protected bool PropsRequired { get; }
        public readonly IEntity ParentOrNull;
        public readonly string FieldOrNull;
        private readonly CodeDataFactory _cdf;

        private readonly Func<bool?> _getDebug;

        public DynamicEntityListHelper(IDynamicEntity singleItem, Func<bool?> getDebug, bool propsRequired, CodeDataFactory cdf)
            : this(cdf, propsRequired, getDebug)
        {
            _list = new List<IDynamicEntity> { singleItem ?? throw new ArgumentException(nameof(singleItem)) };
        }
        
        public DynamicEntityListHelper(IEnumerable<IEntity> entities, IEntity parentOrNull, string fieldOrNull, Func<bool?> getDebug, bool propsRequired, CodeDataFactory cdf)
            : this(cdf, propsRequired, getDebug)
        {
            ParentOrNull = parentOrNull;
            FieldOrNull = fieldOrNull;
            _entities = entities?.ToArray() ?? throw new ArgumentNullException(nameof(entities));
        }

        private DynamicEntityListHelper(CodeDataFactory cdf, bool propsRequired, Func<bool?> getDebug)
        {
            _cdf = cdf ?? throw new ArgumentNullException(nameof(cdf));
            PropsRequired = propsRequired;
            _getDebug = getDebug;
        }
        
        private List<IDynamicEntity> _list;
        private readonly IEntity[] _entities;

        [PrivateApi]
        public List<IDynamicEntity> DynEntities
        {
            get
            {
                // Case #1 & Case #2- already created before or because of Single-Item
                if (_list != null) return _list;

                // Case #3 - Real sub-list
                // If it has a parent, it should apply numbering to the things inside
                // If not, it's coming from a stream or something and shouldn't do that
                var reWrapWithListNumbering = ParentOrNull != null;

                var debug = _getDebug?.Invoke();
                return _list = _entities
                    .Select((e, i) =>
                    {
                        // If we should re-wrap, we create an Entity with some metadata-decoration, so that toolbars know it's part of a list
                        var blockEntity = reWrapWithListNumbering
                            ? EntityInBlockDecorator.Wrap(e, ParentOrNull.EntityGuid, FieldOrNull, i)
                            : e;
                        return SubDataFactory.SubDynEntityOrNull(blockEntity, _cdf, debug, propsRequired: PropsRequired) as IDynamicEntity;
                    })
                    .ToList();
            }
        }

        //public int Count => _entities?.Length ?? 1;

    }
}
