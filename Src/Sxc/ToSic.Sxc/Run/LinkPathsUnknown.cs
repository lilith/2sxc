﻿using System;
using ToSic.Eav.Run;
using ToSic.Eav.Run.Unknown;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Run
{
    internal class LinkPathsUnknown : ILinkPaths, IIsUnknown
    {
        public LinkPathsUnknown(WarnUseOfUnknown<LinkServiceUnknown> _)
        {
            
        }
        
        public string AsSeenFromTheDomainRoot(string virtualPath)
        {
            throw new NotImplementedException();
        }

        // Stub CurrentPage
        public string GetCurrentRequestUrl() => LinkServiceUnknown.NiceCurrentUrl;

        // Stub DomainName
        public string GetCurrentLinkRoot() => LinkServiceUnknown.DefRoot;
    }
}
