﻿using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// Special helper object pass around a url when it started as a string
    /// </summary>
    [PrivateApi]
    public class HasLink: IHasLink
    {
        internal HasLink(string url) => Url = url;

        public string Url { get; }
    }
}
