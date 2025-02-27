﻿using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Code
{
    /// <summary>
    /// Marks objects which have DynCodeRoot which is passed around to sub-objects as needed
    /// </summary>
    [PrivateApi]
    public interface IHasDynamicCodeRoot
    {
        /// <summary>
        /// The dynamic code root which many dynamic code objects need to access prepared context, state etc.
        /// </summary>
        [PrivateApi("internal, for passing around context!")]
        // ReSharper disable once InconsistentNaming
        IDynamicCodeRoot _DynCodeRoot { get; }

    }
}
