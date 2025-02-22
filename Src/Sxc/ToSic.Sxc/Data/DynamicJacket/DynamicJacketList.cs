﻿using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.Json.Nodes;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Data.Wrapper;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// This is a DynamicJacket for JSON arrays. You can enumerate through it. 
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("just use the objects from AsDynamic, don't use this directly")]
    public class DynamicJacketList : DynamicJacketBase<JsonArray>, IReadOnlyList<object>
    {
        /// <inheritdoc />
        internal DynamicJacketList(CodeJsonWrapper wrapper, PreWrapJsonArray preWrap) : base(wrapper, preWrap.GetContents())
        {
            PreWrapList = preWrap;
        }
        private PreWrapJsonArray PreWrapList { get; }

        internal override IPreWrap PreWrap => PreWrapList;

        #region Basic Jacket Properties

        /// <inheritdoc />
        public override bool IsList => true;

        /// <summary>
        /// Count array items or object properties
        /// </summary>
        public override int Count => UnwrappedContents.Count;

        #endregion



        [PrivateApi]
        public override IEnumerator<object> GetEnumerator() 
            => UnwrappedContents.Select(o => Wrapper.IfJsonGetValueOrJacket(o)).GetEnumerator();

        [PrivateApi]
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;
            return true;
        }


        /// <summary>
        /// Access the items in this object - but only if the underlying object is an array. 
        /// </summary>
        /// <param name="index">array index</param>
        /// <returns>the item or an error if not found</returns>
        public override object this[int index] => Wrapper.IfJsonGetValueOrJacket(UnwrappedContents[index]);
    }
}
