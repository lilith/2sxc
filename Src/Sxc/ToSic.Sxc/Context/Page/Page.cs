﻿using ToSic.Lib.DI;
using ToSic.Sxc.Context.Query;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.Parameters;

namespace ToSic.Sxc.Context
{
    public class Page: IPage
    {
        #region Constructor / DI

        /// <summary>
        /// Constructor for DI
        /// </summary>
        public Page(LazySvc<IHttp> httpLazy) => _httpLazy = httpLazy;
        private readonly LazySvc<IHttp> _httpLazy;

        #endregion

        public IPage Init(int id)
        {
            Id = id;
            return this;
        }

        public int Id { get; private set; } = Eav.Constants.NullId;


        public IParameters Parameters => _parameters ?? (_parameters = new Parameters(OriginalParameters.GetOverrideParams(_httpLazy.Value?.QueryStringParams)));
        private IParameters _parameters;


        public string Url { get; set; } = Eav.Constants.UrlNotInitialized;
    }
}
