﻿using System;
using ToSic.Eav.Run.Unknown;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.WebApi.Infrastructure
{
    internal class WebApiContextBuilderUnknown: IWebApiContextBuilder
    {
        public WebApiContextBuilderUnknown(WarnUseOfUnknown<WebApiContextBuilderUnknown> _) { }

        public IContextResolver PrepareContextResolverForApiRequest()
        {
            throw new NotImplementedException();
        }
    }
}
