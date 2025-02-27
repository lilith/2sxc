﻿using System;
using ToSic.Eav.Run.Unknown;
using ToSic.Lib.DI;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Blocks
{
    public class ModuleAndBlockBuilderUnknown: ModuleAndBlockBuilder
    {
        public ModuleAndBlockBuilderUnknown(WarnUseOfUnknown<ModuleAndBlockBuilderUnknown> _, Generator<BlockFromModule>bfmGenerator) : base(bfmGenerator, "Unk")
        {
        }

        protected override IModule GetModuleImplementation(int pageId, int moduleId) => throw new NotSupportedException();

        protected override IContextOfBlock GetContextOfBlock(IModule module, int? pageId) => throw new NotSupportedException();

        protected override IContextOfBlock GetContextOfBlock<TPlatformModule>(TPlatformModule module, int? pageId) => throw new NotSupportedException();
    }
}
