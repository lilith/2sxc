﻿using System.Collections.Generic;
using System.Linq;
using ToSic.Sxc.Engines;

namespace ToSic.Sxc.Web.PageService
{
    public partial class PageServiceShared
    {
        /// <summary>
        /// Assets consolidated from all render-results 
        /// </summary>
        public List<IClientAsset> Assets { get; } = new List<IClientAsset>();


        public void AddAssets(RenderEngineResult result)
        {
            if (result?.Assets == null) return;
            if (!result.Assets.Any()) return;
            Assets.AddRange(result.Assets);
        }

    }
}
