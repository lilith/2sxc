﻿using System;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Blocks
{
    public abstract class ModuleAndBlockBuilder: ServiceBase, IModuleAndBlockBuilder
    {
        private readonly Generator<BlockFromModule> _blockGenerator;

        protected ModuleAndBlockBuilder(Generator<BlockFromModule> blockGenerator, string logPrefix): base($"{logPrefix}.BnMBld")
        {
            ConnectServices(
                _blockGenerator = blockGenerator
            );
        }

        protected abstract IModule GetModuleImplementation(int pageId, int moduleId);

        protected void ThrowIfModuleIsNull<TModule>(int pageId, int moduleId, TModule moduleInfo)
        {
            if (moduleInfo != null) return;
            var msg = $"Can't find module {moduleId} on page {pageId}. Maybe you reversed the ID-order?";
            Log.A(msg);
            throw new Exception(msg);
        }

        public BlockWithContextProvider GetProvider(int pageId, int moduleId)
        {
            var wrapLog = Log.Fn<BlockWithContextProvider>($"{pageId}, {moduleId}");
            var module = GetModuleImplementation(pageId, moduleId);
            var ctx = GetContextOfBlock(module, pageId);
            return wrapLog.ReturnAsOk(new BlockWithContextProvider(ctx, () => _blockGenerator.New().Init(ctx)));
        }

        public BlockWithContextProvider GetProvider<TPlatformModule>(TPlatformModule module, int? page) where TPlatformModule : class
        {
            var ctx = GetContextOfBlock(module, page);
            return new BlockWithContextProvider(ctx, () => _blockGenerator.New().Init(ctx));
        }

        protected abstract IContextOfBlock GetContextOfBlock(IModule module, int? pageId);

        protected abstract IContextOfBlock GetContextOfBlock<TPlatformModule>(TPlatformModule module, int? pageId);
    }
}
