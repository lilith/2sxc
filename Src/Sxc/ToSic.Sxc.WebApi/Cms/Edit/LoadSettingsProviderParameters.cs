﻿using System.Collections.Generic;
using ToSic.Eav.Context;
using ToSic.Eav.Data;

namespace ToSic.Sxc.WebApi.Cms
{
    public class LoadSettingsProviderParameters
    {
        public IContextOfApp ContextOfApp { get; set; }

        public  List<IContentType> ContentTypes { get; set; }

        public List<string> InputTypes { get; set; }
    }
}
