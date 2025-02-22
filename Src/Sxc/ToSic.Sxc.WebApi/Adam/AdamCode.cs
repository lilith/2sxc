﻿using System;
using System.IO;
using ToSic.Eav;
using ToSic.Eav.Configuration;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Code;
using ToSic.Sxc.Services;
using static ToSic.Eav.Configuration.BuiltInFeatures;

namespace ToSic.Sxc.WebApi.Adam
{
    /// <summary>
    /// Adam Shared Code Across the APIs
    /// See docs of official interface <see cref="IDynamicWebApi"/>
    /// </summary>
    [PrivateApi("Used by DynamicApiController and Hybrid.Api12_DynCode")]
    public class AdamCode: ServiceForDynamicCode
    {
        public AdamCode(Generator<AdamTransUpload<int, int>> adamUploadGenerator, LazySvc<IFeaturesInternal> featuresLazy) : base("AdamCode")
        {
            ConnectServices(
                _adamUploadGenerator = adamUploadGenerator,
                _featuresLazy = featuresLazy
            );
        }

        private readonly Generator<AdamTransUpload<int, int>> _adamUploadGenerator;
        private readonly LazySvc<IFeaturesInternal> _featuresLazy;

        public IFile SaveInAdam(string noParamOrder = Parameters.Protector,
            Stream stream = null,
            string fileName = null,
            string contentType = null,
            Guid? guid = null,
            string field = null,
            string subFolder = "")
        {
            Parameters.ProtectAgainstMissingParameterNames(noParamOrder, "SaveInAdam",
                $"{nameof(stream)},{nameof(fileName)},{nameof(contentType)},{nameof(guid)},{nameof(field)},{nameof(subFolder)} (optional)");

            if (stream == null || fileName == null || contentType == null || guid == null || field == null)
                throw new Exception();

            var feats = new[] { SaveInAdamApi.Guid, PublicUploadFiles.Guid };

            if (!_featuresLazy.Value.Enabled(feats, "can't save in ADAM", out var exp))
                throw exp;

            var appId = _DynCodeRoot?.Block?.AppId ?? _DynCodeRoot?.App?.AppId ?? throw new Exception("Error, SaveInAdam needs an App-Context to work, but the App is not known.");
            return _adamUploadGenerator.New()
                .Init(appId, contentType, guid.Value, field, false)
                .UploadOne(stream, fileName, subFolder, true);
        }
    }
}
