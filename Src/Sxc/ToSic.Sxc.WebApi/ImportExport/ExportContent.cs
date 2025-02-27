﻿using System;
using System.Linq;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.Security;
using ToSic.Lib.Logging;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Infrastructure;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps;
using ToSic.Sxc.WebApi.App;
#if NETFRAMEWORK
using THttpResponseType = System.Net.Http.HttpResponseMessage;
#else
using THttpResponseType = Microsoft.AspNetCore.Mvc.IActionResult;
#endif

namespace ToSic.Sxc.WebApi.ImportExport
{
    public class ExportContent : ServiceBase
    {
        #region Constructor / DI

        public ExportContent(XmlExporter xmlExporter, LazySvc<CmsRuntime> cmsRuntime, ISite site, IUser user, Generator<ImpExpHelpers> impExpHelpers, IResponseMaker responseMaker)
            : base("Bck.Export") =>
            ConnectServices(
                _xmlExporter = xmlExporter,
                _cmsRuntime = cmsRuntime,
                _site = site,
                _user = user,
                _impExpHelpers = impExpHelpers,
                _responseMaker = responseMaker
            );

        private readonly XmlExporter _xmlExporter;
        private readonly LazySvc<CmsRuntime> _cmsRuntime;
        private readonly ISite _site;
        private CmsRuntime CmsRuntime => _cmsRuntime.Value;
        private readonly IUser _user;
        private readonly Generator<ImpExpHelpers> _impExpHelpers;
        private readonly IResponseMaker _responseMaker;

        #endregion

        public ExportPartsOverviewDto PreExportSummary(int zoneId, int appId, string scope)
        {
            Log.A($"get content info for z#{zoneId}, a#{appId}, scope:{scope} super?:{_user.IsSystemAdmin}");
            var contextZoneId = _site.ZoneId;
            var currentApp = _impExpHelpers.New().GetAppAndCheckZoneSwitchPermissions(zoneId, appId, _user, contextZoneId);

            var cms = CmsRuntime.InitQ(currentApp);
            var contentTypes = cms.ContentTypes.All.OfScope(scope);
            var entities = cms.Entities.All;
            var templates = cms.Views.GetAll();

            return new ExportPartsOverviewDto
            {
                ContentTypes = contentTypes.Select(c => new ExportPartsContentTypesDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    StaticName = c.NameId,
                    Templates = templates.Where(t => t.ContentType == c.NameId)
                        .Select(t => new IdNameDto
                        {
                            Id = t.Id,
                            Name = t.Name
                        }),
                    Entities = entities
                        .Where(e => e.Type.Id == c.Id)
                        .Select(e => new ExportPartsEntitiesDto
                        {
                            Title = e.GetBestTitle(),
                            Id = e.EntityId
                        })
                }),
                TemplatesWithoutContentTypes = templates
                    .Where(t => string.IsNullOrEmpty(t.ContentType))
                    .Select(t => new IdNameDto
                    {
                        Id = t.Id,
                        Name = t.Name
                    })
            };
        }


        public THttpResponseType Export(int zoneId, int appId, string contentTypeIdsString, string entityIdsString,
            string templateIdsString)
        {
            Log.A($"export content z#{zoneId}, a#{appId}, ids:{entityIdsString}, templId:{templateIdsString}");
            SecurityHelpers.ThrowIfNotSiteAdmin(_user, Log); // must happen inside here, as it's opened as a new browser window, so not all headers exist

            var contextZoneId = _site.ZoneId;
            var currentApp = _impExpHelpers.New().GetAppAndCheckZoneSwitchPermissions(zoneId, appId, _user, contextZoneId);
            var appRuntime = CmsRuntime.InitQ(currentApp);

            var fileName = $"2sxcContentExport_{currentApp.NameWithoutSpecialChars()}_{currentApp.VersionSafe()}.xml";
            var fileXml = _xmlExporter.Init(zoneId, appId, appRuntime, false,
                contentTypeIdsString?.Split(';') ?? Array.Empty<string>(),
                entityIdsString?.Split(';') ?? Array.Empty<string>()
            ).GenerateNiceXml();

            return _responseMaker.File(fileXml, fileName, "text/xml");
        }
    }
}
