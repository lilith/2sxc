﻿using System;
using ToSic.Eav.Apps.Run;
using ToSic.Lib.Logging;
using ToSic.Eav.Run;
using ToSic.Eav.Run.Unknown;

namespace ToSic.Sxc.Context
{
    public class ModuleUnknown: IModule, IIsUnknown
    {
        // ReSharper disable once UnusedParameter.Local
        public ModuleUnknown(WarnUseOfUnknown<ModuleUnknown> _) { }

        public IModule Init(int id)
        {
            // don't do anything
            return this;
        }

        public int Id => Eav.Constants.NullId;
        public bool IsContent => true;

        public IBlockIdentifier BlockIdentifier =>
            new BlockIdentifier(Eav.Constants.NullId, Eav.Constants.NullId, Eav.Constants.NullNameId, Guid.Empty, Guid.Empty);
    }
}
