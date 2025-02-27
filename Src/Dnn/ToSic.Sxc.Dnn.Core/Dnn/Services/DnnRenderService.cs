﻿using System.Web;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Web;
using Page = System.Web.UI.Page;

namespace ToSic.Sxc.Dnn.Services
{
    public class DnnRenderService : RenderService
    {
        private readonly LazySvc<DnnPageChanges> _dnnPageChanges;
        private readonly LazySvc<DnnClientResources> _dnnClientResources;
        private readonly Generator<IContextOfBlock> _context;

        public DnnRenderService(
            MyServices services,
            LazySvc<DnnPageChanges> dnnPageChanges,
            LazySvc<DnnClientResources> dnnClientResources,
            Generator<IContextOfBlock> context
        ) : base(services)
        {
            ConnectServices(
                _dnnPageChanges = dnnPageChanges,
                _dnnClientResources = dnnClientResources,
                _context = context
            );
        }

        public override IRenderResult Module(int pageId, int moduleId,
            string noParamOrder = Eav.Parameters.Protector,
            object data = null)
        {
            var l = Log.Fn<IRenderResult>($"{nameof(pageId)}: {pageId}, {nameof(moduleId)}: {moduleId}");
            var result = base.Module(pageId, moduleId, noParamOrder, data);

            // this code should be executed in PreRender of page (ensure when calling) or it is too late
            if (HttpContext.Current?.Handler is Page dnnHandler) // detect if we are on the page
                if (_context.New().Module.BlockIdentifier == null) // find if is in module (because in module it's already handled)
                    DnnPageProcess(dnnHandler, result);

            return l.ReturnAsOk(result);
        }

        private void DnnPageProcess(Page dnnPage, IRenderResult result)
        {
            _dnnPageChanges.Value.Apply(dnnPage, result);
            _dnnClientResources.Value.Init(dnnPage, null, null).AddEverything(result.Features);
        }
    }
}
