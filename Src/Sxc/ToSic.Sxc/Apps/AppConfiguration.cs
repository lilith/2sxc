﻿using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
// ReSharper disable UnusedMember.Global - we need these, as it's a public API

namespace ToSic.Sxc.Apps
{
    /// <summary>
    /// The configuration of the app, as you can set it in the app-package definition.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public class AppConfiguration: EntityBasedWithLog
    {
        // todo: probably move most to Eav.Apps.AppConstants
        [PrivateApi]
        public const string
            FieldDescription = "Description",
            //FieldName = "DisplayName",
            //FieldFolder = "Folder",
            FieldOriginalId = "OriginalId",
            FieldVersion = "Version",
            FieldAllowRazor = "AllowRazorTemplates",
            FieldAllowToken = "AllowTokenTemplates",
            //FieldHidden = "Hidden",
            FieldRequiredSxcVersion = "RequiredVersion",
            FieldRequiredDnnVersion = "RequiredDnnVersion",
            FieldRequiredOqtaneVersion = "RequiredOqtaneVersion",
            FieldSupportsAjax = "SupportsAjaxReload";

        [PrivateApi]
        public AppConfiguration(IEntity entity, ILog parentLog) : base(entity, parentLog, "Sxc.AppCnf")
        {
        }

        public Version Version
        {
            get
            {
                var versionValue = Get(FieldVersion, "");
                var valid = Version.TryParse(versionValue, out var version);
                return valid
                    ? version
                    : new Version();
            }
        }

        public string Name => Get(AppConstants.FieldName, Eav.Constants.NullNameId);

        public string Description => Get(FieldDescription, "");

        public string Folder => Get( AppConstants.FieldFolder, "");

        public bool EnableRazor => Get(FieldAllowRazor, false);

        public bool EnableToken => Get(FieldAllowToken, false);

        public bool IsHidden => Get(AppConstants.FieldHidden, false);

        public bool EnableAjax => Get(FieldSupportsAjax, false);

        public Guid OriginalId => Guid.TryParse(Get(FieldOriginalId, ""), out var result) ? result : Guid.Empty;

        public Version RequiredSxc =>
            Version.TryParse(Get(FieldRequiredSxcVersion, ""), out var version)
                ? version
                : new Version();

        public  Version RequiredDnn =>
            Version.TryParse(Get(FieldRequiredDnnVersion, ""), out var version)
                ? version
                : new Version();
        
        public Version RequiredOqtane => Version.TryParse(Get(FieldRequiredOqtaneVersion, ""), out var version)
            ? version
            : new Version();
    }
}
