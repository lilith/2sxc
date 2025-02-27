﻿using ToSic.Lib.Logging;


namespace ToSic.Sxc.Run
{
    public interface IEnvironmentInstaller: IHasLog
    {
        /// <summary>
        /// Get upgrade messages to show to the user if the upgrade/install needs attention
        /// </summary>
        /// <returns></returns>
        string UpgradeMessages();

        /// <summary>
        /// Manually trigger continue-update
        /// </summary>
        /// <returns></returns>
        bool ResumeAbortedUpgrade();

        //string GetAutoInstallPackagesUiUrl(ISite site, IModule module, bool forContentApp);
    }
}
