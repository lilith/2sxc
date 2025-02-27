﻿using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Context;
using ToSic.Lib.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;
using static ToSic.Eav.Configuration.ConfigurationStack;
using BuiltInFeatures = ToSic.Sxc.Configuration.Features.BuiltInFeatures;
using IFeaturesService = ToSic.Sxc.Services.IFeaturesService;

namespace ToSic.Sxc.Web.ContentSecurityPolicy
{
    public  class CspOfModule: ServiceForDynamicCode
    {
        #region Constructor

        public CspOfModule(IUser user, IFeaturesService featuresService): base($"{CspConstants.LogPrefix}.ModLvl")
        {
            _user = user;
            _featuresService = featuresService;
        }

        private readonly IUser _user;
        private readonly IFeaturesService _featuresService;

        ///// <summary>
        ///// Connect to code root, so page-parameters and settings will be available later on.
        ///// Important: page-parameters etc. are not available at this time, so don't try to get them until needed
        ///// </summary>
        ///// <param name="codeRoot"></param>
        //public override void ConnectToRoot(IDynamicCodeRoot codeRoot)
        //{
        //    if (_alreadyConnected) return;
        //    _alreadyConnected = true;
        //    (Log as Log)?.LinkTo(codeRoot.Log);
        //    _codeRoot = codeRoot;
        //    Log.Fn().Done();
        //}

        //private bool _alreadyConnected;

        //private IDynamicCodeRoot _codeRoot;
        private DynamicStack CodeRootSettings()
        {
            var stack = _DynCodeRoot?.Settings as DynamicStack;
            // Enable this for detailed debugging
            //if (stack != null) stack.Debug = true;
            return stack;
        }

        #endregion

        #region App Level CSP Providers

        /// <summary>
        /// Each App will register itself here to be added to the CSP list
        /// </summary>
        private List<CspOfApp> AppCsps = new List<CspOfApp>();

        internal bool RegisterAppCsp(CspOfApp appCsp)
        {
            var cLog = Log.Fn<bool>($"appId: {appCsp?.AppId}");
            if (appCsp == null)
                return cLog.ReturnFalse("null");

            // Note: We tried not-adding duplicates but this doesn't work
            // Because at the moment of registration, the AppId is often not known yet
            // Do not delete this comment, as others will attempt this too
            //if (AppCsps.Any(a => a.AppId == appCsp.AppId)) 
            //    return cLog.Done($"app {appCsp.AppId} exists", false);
            AppCsps.Add(appCsp);
            return cLog.ReturnTrue("added");
        }

        #endregion

        #region Url Parameters to Detect Dev / True

        public bool UrlIsDevMode => _urlDevMode.Get(() => CspUrlParam.EqualsInsensitive(CspConstants.CspUrlDev));
        private readonly GetOnce<bool> _urlDevMode = new GetOnce<bool>();

        private string CspUrlParam => _cspUrlParam.Get(Log, () =>
        {
            if (!_featuresService.IsEnabled(BuiltInFeatures.ContentSecurityPolicyTestUrl.NameId))
                return null;
            var pageParameters = _DynCodeRoot?.CmsContext?.Page?.Parameters;
            if (pageParameters == null) return null;
            pageParameters.TryGetValue(CspConstants.CspUrlParameter, out var cspParam);
            return cspParam;
        });
        private readonly GetOnce<string> _cspUrlParam = new GetOnce<string>();

        #endregion

        #region Read Settings

        /// <summary>
        /// CSP Settings Reader from Dynamic Entity for the Site
        /// </summary>
        private CspSettingsReader SiteCspSettings => _siteCspSettings.Get(Log, () =>
        {
            var pageSettings = CodeRootSettings()?.GetStack(PartSiteSystem, PartGlobalSystem, PartPresetSystem);
            return new CspSettingsReader(pageSettings, _user, UrlIsDevMode, Log);
        });
        private readonly GetOnce<CspSettingsReader> _siteCspSettings = new GetOnce<CspSettingsReader>();

        #endregion

        #region Enabled / Enforced

        /// <summary>
        /// Enforce?
        /// </summary>
        internal bool IsEnforced => _cspReportOnly.Get(Log, () => SiteCspSettings.IsEnforced);
        private readonly GetOnce<bool> _cspReportOnly = new GetOnce<bool>();


        /// <summary>
        /// Check if enabled based on various criteria like features, url-param, settings etc.
        /// </summary>
        internal bool IsEnabled => _enabled.Get(Log, () =>
        {
            // Check features
            if (!_featuresService.IsEnabled(BuiltInFeatures.ContentSecurityPolicy.NameId))
                return false;
            if(_featuresService.IsEnabled(BuiltInFeatures.ContentSecurityPolicyEnforceTemp.NameId))
                return true;

            // Try settings
            if (SiteCspSettings.IsEnabled) 
                return true;

            // Check URL Parameters - they are null if the feature is not enabled
            return CspUrlParam.EqualsInsensitive(CspConstants.CspUrlTrue) || UrlIsDevMode;
        });
        private readonly GetOnce<bool> _enabled = new GetOnce<bool>();


        #endregion


        private List<KeyValuePair<string, string>> Policies => _policies.Get(Log, () =>
        {
            var sitePolicies = SiteCspSettings.Policies;
            Log.A($"Site.Policies: {sitePolicies}");

            var appPolicies = GetAppPolicies();
            var merged = $"{sitePolicies}\n{appPolicies}";
            Log.A($"Merged: {merged}");
            return new CspPolicyTextProcessor(Log).Parse(merged);
        });
        private readonly GetOnce<List<KeyValuePair<string, string>>> _policies = new GetOnce<List<KeyValuePair<string, string>>>();

        private string GetAppPolicies()
        {
            var cLog = Log.Fn<string>();

            var deduplicate = AppCsps
                .GroupBy(ac => ac.AppId)
                .Select(g => g.First())
                .ToList();


            var appPolicySets = deduplicate
                .Select(ac =>
                {
                    var p = ac.AppPolicies;
                    Log.A($"App[{ac.AppId}]: {p}");
                    return p.NullIfNoValue();
                })
                .Where(p => p.HasValue())
                .ToList();

            var appPolicies = string.Join("\n", appPolicySets);

            return cLog.Return(appPolicies, $"Total: {AppCsps.Count}; Distinct: {deduplicate.Count}; With Value: {appPolicySets.Count}");
        }



        internal void AddCspService(ContentSecurityPolicyServiceBase provider) => CspServices.Add(provider);
        internal readonly List<ContentSecurityPolicyServiceBase> CspServices = new List<ContentSecurityPolicyServiceBase>();

        public List<CspParameters> CspParameters()
        {
            var wrapLog = Log.Fn<List<CspParameters>>();
            if (!IsEnabled) return wrapLog.Return(new List<CspParameters>(), "disabled");

            if (Policies.Any())
            {
                Log.A("Policies found");
                // Create a CspService which just contains these new policies for merging later on
                var policyCsp = new ContentSecurityPolicyServiceBase();
                foreach (var policy in Policies)
                    policyCsp.Add(policy.Key, policy.Value);
                AddCspService(policyCsp);
            }

            if (!CspServices.Any()) return wrapLog.Return(new List<CspParameters>(), "no services to add");
            var result = CspServices.Select(c => c?.Policy).Where(c => c != null).ToList();
            return wrapLog.ReturnAsOk(result);

        }
        
    }
}
