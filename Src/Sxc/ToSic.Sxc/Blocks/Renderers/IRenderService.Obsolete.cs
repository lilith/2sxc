﻿using System;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
using ToSic.Razor.Markup;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Blocks
{
    // Important: There is a critical bug in Razor that methods which an interface inherits
    // Will fail when called using dynamic parameters.
    // https://stackoverflow.com/questions/3071634/strange-behaviour-when-using-dynamic-types-as-method-parameters
    // Because of this,
    // - ToSic.Sxc.Web.IPageService.SetTitle("ok") works
    // - ToSic.Sxc.Web.IPageService.SetTitle(dynEntity.Title) fails!!!
    // This is why each method on the underlying interface must be repeated here :(
    //
    // We suggest that we won't do this for new commands, but all commands that were in 12.08 must be repeated here


    /// <summary>
    /// Old name for the IRenderService, it's in use in some v12 App templates so we must keep it working.
    /// Will continue to work, but shouldn't be used. Please use <see cref="ToSic.Sxc.Services.IRenderService"/>  instead
    /// </summary>
    [Obsolete("Old name, used in 2-3 v12 apps released. Pls use ToSic.Sxc.Services.IRenderService instead.")]
    public interface IRenderService: ToSic.Sxc.Services.IRenderService
    {
        [PrivateApi]
//#pragma warning disable CS0108, CS0114
        new IRawHtmlString One(ICanBeItem parent, string noParamOrder = Eav.Parameters.Protector,
            ICanBeEntity item = null, object data = null, string field = null, Guid? newGuid = null);
        [PrivateApi]
        new IRawHtmlString All(ICanBeItem parent, string noParamOrder = Eav.Parameters.Protector,
            string field = null, string apps = null, int max = 100, string merge = null);
//#pragma warning restore CS0108, CS0114
    }
}
