﻿using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Services;
using static ToSic.Eav.Parameters;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    [PrivateApi("This will already be documented through the Dnn DLL so shouldn't appear again in the docs")]
    // ReSharper disable once UnusedMember.Global
    public abstract class RazorTyped: OqtRazorBase<dynamic>, IHasCodeLog, IRazor, ISetDynamicModel, IDynamicCode16
    {
        #region Constructor / DI / SysHelp

        /// <summary>
        /// Constructor - only available for inheritance
        /// </summary>
        [PrivateApi]
        protected RazorTyped() : base(ToSic.Sxc.Constants.CompatibilityLevel16, "Oqt.Rzr16") { }

        #endregion

        #region ServiceKit

        /// <inheritdoc cref="IDynamicCode16.Kit"/>
        public ServiceKit16 Kit => _kit.Get(() => _DynCodeRoot.GetKit<ServiceKit16>());
        private readonly GetOnce<ServiceKit16> _kit = new GetOnce<ServiceKit16>();

        #endregion

        #region MyModel

        [PrivateApi("WIP v16.02")]
        public ITypedModel MyModel => CodeHelper.MyModel;

        #endregion

        #region New App, Settings, Resources

        /// <inheritdoc cref="IDynamicCode.Link" />
        public ILinkService Link => _DynCodeRoot.Link;

        /// <inheritdoc />
        public IAppTyped App => (IAppTyped)_DynCodeRoot.App;

        /// <inheritdoc cref="IDynamicCode16.AllResources" />
        public ITypedStack AllResources => CodeHelper.AllResources;

        /// <inheritdoc cref="IDynamicCode16.AllSettings" />
        public ITypedStack AllSettings => CodeHelper.AllSettings;

        #endregion

        #region My... Stuff

        private TypedCode16Helper CodeHelper => SysHlp.CodeHelper;

        public ITypedItem MyItem => CodeHelper.MyItem;

        public IEnumerable<ITypedItem> MyItems => CodeHelper.MyItems;

        public ITypedItem MyHeader => CodeHelper.MyHeader;

        public IContextData MyData => _DynCodeRoot.Data;

        #endregion

        #region As Conversions

        /// <inheritdoc cref="IDynamicCode16.AsItem" />
        public ITypedItem AsItem(object data, string noParamOrder = Protector, bool? propsRequired = default, bool? mock = default)
            => _DynCodeRoot.Cdf.AsItem(data, noParamOrder, propsRequired: propsRequired ?? true, mock: mock);

        /// <inheritdoc cref="IDynamicCode16.AsItems" />
        public IEnumerable<ITypedItem> AsItems(object list, string noParamOrder = Protector, bool? propsRequired = default)
            => _DynCodeRoot.Cdf.AsItems(list, noParamOrder, propsRequired: propsRequired ?? true);

        /// <inheritdoc cref="IDynamicCode16.AsEntity" />
        public IEntity AsEntity(ICanBeEntity thing) => _DynCodeRoot.Cdf.AsEntity(thing);

        /// <inheritdoc cref="IDynamicCode16.AsTyped" />
        public ITyped AsTyped(object original, string noParamOrder = Protector, bool? propsRequired = default)
            => _DynCodeRoot.Cdf.AsTyped(original, propsRequired: propsRequired);

        /// <inheritdoc cref="IDynamicCode16.AsTypedList" />
        public IEnumerable<ITyped> AsTypedList(object list, string noParamOrder = Protector, bool? propsRequired = default)
            => _DynCodeRoot.Cdf.AsTypedList(list, noParamOrder, propsRequired: propsRequired);

        /// <inheritdoc cref="IDynamicCode16.AsStack" />
        public ITypedStack AsStack(params object[] items) => _DynCodeRoot.Cdf.AsStack(items);

        #endregion


        /// <inheritdoc cref="IDynamicCode16.GetCode"/>
        public dynamic GetCode(string path, string noParamOrder = Protector, string className = default) => SysHlp.GetCode(path, noParamOrder, className);

        #region MyContext

        /// <inheritdoc cref="IDynamicCode16.MyContext" />
        public ICmsContext MyContext => _DynCodeRoot.CmsContext;

        /// <inheritdoc cref="IDynamicCode16.MyUser" />
        public ICmsUser MyUser => _DynCodeRoot.CmsContext.User;

        /// <inheritdoc cref="IDynamicCode16.MyPage" />
        public ICmsPage MyPage => _DynCodeRoot.CmsContext.Page;

        /// <inheritdoc cref="IDynamicCode16.MyView" />
        public ICmsView MyView => _DynCodeRoot.CmsContext.View;

        #endregion

    }
}
