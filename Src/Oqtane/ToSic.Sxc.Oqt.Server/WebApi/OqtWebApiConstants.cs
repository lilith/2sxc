﻿using System.Collections.Generic;
using System.Text.RegularExpressions;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Web.EditUi;

namespace ToSic.Sxc.Oqt.Server.WebApi
{
    public static class OqtWebApiConstants
    {
        public const string Auto = "auto";

        // Release routes
        // Names are a bit strange so they are the same length
        // Which helps align values when they are used together later on

        public const string RootNoLanguage = "";
        public const string RootPathOrLang = "{path-or-language}/";
        public const string RootPathNdLang = "{path}/{subpath-or-language}/";

        public const string ApiRootWithNoLang = RootNoLanguage + "api/sxc";
        public const string ApiRootPathOrLang = RootPathOrLang + "api/sxc";
        public const string ApiRootPathNdLang = RootPathNdLang + "api/sxc";
        
        public const string AppRootNoLanguage = RootNoLanguage + "app";
        public const string AppRootPathOrLang = RootPathOrLang + "app";
        public const string AppRootPathNdLang = RootPathNdLang + "app";

        public const string SharedRootNoLanguage = RootNoLanguage + "2sxc/shared";
        public const string SharedRootPathOrLang = RootPathOrLang + "2sxc/shared";
        public const string SharedRootPathNdLang = RootPathNdLang + "2sxc/shared";

        // Beta routes
        //public const string WebApiStateRoot = "{alias:int}/api/sxc";

        // Endpoint mappings
        public static readonly string[] SxcEndpointPatterns = new[]
        {
            // Release routes
            AppRootNoLanguage + "/{appFolder}/api/{controller}/{action}",
            AppRootNoLanguage + "/{appFolder}/{edition}/api/{controller}/{action}",
            AppRootPathOrLang + "/{appFolder}/api/{controller}/{action}",
            AppRootPathOrLang + "/{appFolder}/{edition}/api/{controller}/{action}",
            AppRootPathNdLang + "/{appFolder}/api/{controller}/{action}",
            AppRootPathNdLang + "/{appFolder}/{edition}/api/{controller}/{action}",
            //// Beta routes
            //WebApiStateRoot + "/app/{appFolder}/api/{controller}/{action}",
            //WebApiStateRoot + "/app/{appFolder}/{edition}/api/{controller}/{action}"
        };

        // Regex patterns to match endpoint mappings
        // This regular expression is for matching URL paths to 2sxc app custom endpoints.
        // For example, URL like this: "/subportal/language/app/custom-2sxc-app-name/edition-name/api/MyCtr/Method".
        // The given regular expression, `(.*/)?app/([\w-]+)(/([\w-]+))?/api/(.+)`, can be broken down as follows:
        // 1. `(.*/)?` : Optional 'alias' and/or 'language' (path ending with /)
        //               This captures zero or one occurrence of any characters (except a new line), ending with a forward slash('/').
        //               This part is optional because of the '?' after the group, which means it could match strings like "subportal/" or
        //               "subportal/language/" etc. and also an empty string.
        // 2. `app/` : This simply matches the exact string 'app/'.
        // 3. `([\w -]+)` : 'custom 2sxc app name'
        //                  This captures zero or more word characters (letters, numbers, underscores) or hyphens.
        //                  This part could match strings like "example-app", "blog6", "app3", etc., but not an empty string.
        // 4. `(/([\w -]+))?` : Optional one path segement with 2sxc app '/edition name',
        //                      This is an optional part, meaning it could match an empty string.
        //                      If present, it must start with a forward slash('/') followed by one or more word characters(letters, numbers, underscores) or hyphens.
        //                      This part could match strings like "/live", "/bd5", etc., but not a single '/'.
        // 5. `/api/` : This simply matches the exact string '/api/'.
        // 6. `(.+)` : API endpoint path 'controller/action'
        //             This captures one or more of any characters (except a new line).
        //             This part must be present and could match strings like "get-data", "123", "user/data", etc.
        public static readonly Regex SxcEndpointPathRegex = new Regex(@"(.*/)?app/([\w-]+)(/([\w-]+))?/api/(.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // Regex patterns to match endpoint mappings
        //public static readonly string[] SxcEndpointPathRegexPatterns = new[]
        //{
        //    // Release routes
        //    @"(.*/)?app/([\w-]*)(/([\w-]*))?/api/(.+)",
        //    //// Beta routes
        //    //@"(\d+)/api/sxc/app/([\w-]*)/api/([\w-]*)/([\w-]*)",
        //    //@"(\d+)/api/sxc/app/([\w-]*)/([\w-]*)/api/([\w-]*)/([\w-]*)"
        //};

        // Dialogs for 2sxc UI
        public static readonly List<(string url, string page, EditUiResourceSettings setting)> SxcDialogs = new(capacity: 2)
        {
            ($"/Modules/{OqtConstants.PackageName}/dist/quickDialog/", $@"Modules\{OqtConstants.PackageName}\dist\quickDialog\index-raw.html", EditUiResourceSettings.QuickDialog),
            ($"/Modules/{OqtConstants.PackageName}/dist/ng-edit/", $@"Modules\{OqtConstants.PackageName}\dist\ng-edit\index-raw.html", EditUiResourceSettings.EditUi)
        };
    }
}
