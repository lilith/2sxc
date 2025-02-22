﻿using System.Collections.Generic;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSources;
using ToSic.Lib.Documentation;
using ToSic.Sxc.DataSources;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// The main data source for Blocks (Razor, WebApi).
    /// Internally often uses <see cref="CmsBlock"/> to find what it should provide.
    /// It's based on the <see cref="PassThrough"/> data source, because it's just a wrapper.
    /// </summary>
    /// <remarks>
    /// Introduced in v16.01 to simplify the API when using <see cref="ITypedItem"/>s.
    /// </remarks>
    [PublicApi]
    public interface IContextData: IDataSource // IBlockDataSource
    {
        [PrivateApi("maybe just add for docs")]
        new IReadOnlyDictionary<string, IDataStream> Out { get; }
    }
}
