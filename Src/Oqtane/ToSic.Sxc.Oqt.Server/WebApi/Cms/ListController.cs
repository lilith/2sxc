﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System;
using ToSic.Eav.WebApi.Cms;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Oqt.Server.Controllers;
using RealController = ToSic.Sxc.WebApi.Cms.ListControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Cms
{
    // Release routes
    [Route(OqtWebApiConstants.ApiRootWithNoLang + $"/{AreaRoutes.Cms}")]
    [Route(OqtWebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Cms}")]
    [Route(OqtWebApiConstants.ApiRootPathNdLang + $"/{AreaRoutes.Cms}")]

    [ValidateAntiForgeryToken]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [Authorize(Roles = RoleNames.Admin)]
    public class ListController : OqtStatefulControllerBase, IListController
    {
        public ListController(): base(RealController.LogSuffix) { }

        private RealController Real => GetService<RealController>();


        /// <inheritdoc />
        /// <summary>
        /// used to be GET Module/ChangeOrder
        /// </summary>
        [HttpPost]
        public void Move(Guid? parent, string fields, int index, int toIndex) 
            => Real.Move(parent, fields, index, toIndex);

        /// <inheritdoc />
        /// <summary>
        /// Used to be Get Module/RemoveFromList
        /// </summary>
        [HttpDelete]
        public void Delete(Guid? parent, string fields, int index) 
            => Real.Delete(parent, fields, index);
    }
}