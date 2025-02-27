﻿using ToSic.Lib.Documentation;
using ToSic.Sxc.Code;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    [PrivateApi("not sure where/if it goes anywhere")]
    public interface IRazor: IHasDynamicCodeRoot, INeedsDynamicCodeRoot
    {
        /// <summary>
        /// The path to this Razor WebControl.
        /// This is for consistency, because asp.net Framework has a property "VirtualPath" whereas .net core uses "Path"
        /// From now on it should always be Path for cross-platform code
        /// </summary>
        [PublicApi("This is a polyfill to ensure the old Razor has the same property as .net Core Razor")]
        string Path { get; }

    }
}
