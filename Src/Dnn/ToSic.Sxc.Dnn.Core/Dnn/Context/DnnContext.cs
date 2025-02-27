﻿using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Run;

namespace ToSic.Sxc.Dnn.Context
{
    [PrivateApi("this is just internal, external users don't really have anything to do with this")]
    public class DnnContext : IDnnContext, INeedsDynamicCodeRoot
    {
        /// <summary>
        /// Build DNN Helper
        /// Note that the context can be null, in which case it will have no module context, and default to the current portal
        /// </summary>
        public void ConnectToRoot(IDynamicCodeRoot codeRoot)
        {
            var moduleContext = codeRoot.Block?.Context?.Module;
            Module = (moduleContext as Module<ModuleInfo>)?.GetContents();
            // note: this may be a bug, I assume it should be Module.OwnerPortalId
            Portal = PortalSettings.Current ?? 
                (moduleContext != null ? new PortalSettings(Module.PortalID): null);
        }

        public ModuleInfo Module { get; private set; }

        public TabInfo Tab => Portal?.ActiveTab;

        public PortalSettings Portal { get; private set; }

        public UserInfo User => Portal.UserInfo;
    }
}