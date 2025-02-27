﻿using System;
using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;

// Since DynamicEntity... is a wrapper,
// These things ensure that various standalone wrappers are still regarded as equals
// If the underlying entity is the same
namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity: IEquatable<IEntityWrapper>
    {
        [PrivateApi] public IEntity RootContentsForEqualityCheck { get; private set; }
        [PrivateApi] public List<IDecorator<IEntity>> Decorators { get; private set; }

        #region Changing comparison operation to internally compare the entities, not this wrapper

        [PrivateApi]
        public static bool operator ==(DynamicEntity d1, IEntityWrapper d2) => WrapperEquality.IsEqual(d1, d2);

        [PrivateApi]
        public static bool operator !=(DynamicEntity d1, IEntityWrapper d2) => !WrapperEquality.IsEqual(d1, d2);

        [PrivateApi]
        public bool Equals(IEntityWrapper other) => WrapperEquality.EqualsWrapper(this, other);

        /// <inheritdoc />
        [PrivateApi]
        public override bool Equals(object obj) => WrapperEquality.EqualsObj(this, obj);

        /// <summary>
        /// This is used by various equality comparison. 
        /// Since we define two DynamicEntities to be equal when they host the same entity, this uses the Entity.HashCode
        /// </summary>
        /// <returns></returns>
        [PrivateApi]
#pragma warning disable RS1024 // Compare symbols correctly
        public override int GetHashCode() => WrapperEquality.GetHashCode(this);
#pragma warning restore RS1024 // Compare symbols correctly

        /// <inheritdoc />
        [PrivateApi]
        public bool Equals(IDynamicEntity dynObj) => WrapperEquality.EqualsWrapper(this, dynObj);

        #endregion
    }
}
