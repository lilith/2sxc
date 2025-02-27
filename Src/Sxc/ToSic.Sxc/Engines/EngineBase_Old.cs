﻿#if NETFRAMEWORK
using System;
using System.Collections.Generic;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Search;

namespace ToSic.Sxc.Engines
{
    public abstract partial class EngineBase
    {
#pragma warning disable CS0612
        [PrivateApi] protected Purpose Purpose = Purpose.WebView;
#pragma warning restore CS0612


#pragma warning disable CS0612
        /// <inheritdoc />
        public void Init(IBlock block, Purpose purpose) => Log.Do($"{nameof(purpose)}:{purpose}", () =>
        {
            Purpose = purpose;
            Init(block);
        });
#pragma warning restore CS0612


        /// <inheritdoc />
        [Obsolete("Shouldn't be used any more, but will continue to work for indefinitely for old base classes, not in v12. There are now better ways of doing this")]
        [PrivateApi]
        public virtual void CustomizeData() { }

        /// <inheritdoc />
        [Obsolete("Shouldn't be used any more, but will continue to work for indefinitely for old base classes, not in v12. There are now better ways of doing this")]
        [PrivateApi]
        public virtual void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IModule moduleInfo,
            DateTime beginDate)
        {
        }

    }
}
#endif
