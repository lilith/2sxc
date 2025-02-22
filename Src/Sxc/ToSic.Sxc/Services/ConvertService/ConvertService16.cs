﻿using System;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Services;
using static ToSic.Eav.Parameters;

// ReSharper disable MethodOverloadWithOptionalParameter

namespace ToSic.Sxc.Services
{
    [PrivateApi("Hide implementation")]
    internal class ConvertService16: ServiceBase, IConvertService16
    {
        private readonly LazySvc<ConvertForCodeService> _code;
        private readonly ConvertValueService _cnvSvc;

        public ConvertService16(ConvertValueService cnvSvc, LazySvc<ConvertForCodeService> code, LazySvc<IJsonService> json): base("Sxc.CnvSrv")
        {
            ConnectServices(
                _cnvSvc = cnvSvc,
                _code = code,
                _jsonLazy = json
            );
        }

        public bool OptimizeNumbers => true;

        public bool OptimizeBoolean => true;

        public T To<T>(object value) => value.ConvertOrDefault<T>(numeric: OptimizeNumbers, truthy: OptimizeBoolean);

        public T To<T>(object value, string noParamOrder = Protector, T fallback = default) => _cnvSvc.To(value, noParamOrder, fallback);

        public int ToInt(object value) => _cnvSvc.To<int>(value);
        public int ToInt(object value, string noParamOrder = Protector, int fallback = 0) => _cnvSvc.To(value, fallback: fallback);

        public Guid ToGuid(object value) => _cnvSvc.To<Guid>(value);
        public Guid ToGuid(object value, string noParamOrder = Protector, Guid fallback = default) => _cnvSvc.To(value, fallback: fallback);

        public float ToFloat(object value) => _cnvSvc.To<float>(value);
        public float ToFloat(object value, string noParamOrder = Protector, float fallback = default) => _cnvSvc.To(value, fallback: fallback);

        public decimal ToDecimal(object value) => _cnvSvc.To<decimal>(value);
        public decimal ToDecimal(object value, string noParamOrder = Protector, decimal fallback = default) => _cnvSvc.To(value, fallback: fallback);

        public double ToDouble(object value) => _cnvSvc.To<double>(value);
        public double ToDouble(object value, string noParamOrder = Protector, double fallback = default) => _cnvSvc.To(value, fallback: fallback);

        public bool ToBool(object value) => _cnvSvc.To<bool>(value);
        public bool ToBool(object value, string noParamOrder = Protector, bool fallback = false) => _cnvSvc.To(value, fallback: fallback);
        
        public string ToString(object value) => _cnvSvc.To<string>(value);

        public string ToString(object value, string noParamOrder = Protector, string fallback = default, bool fallbackOnNull = true) 
            => _cnvSvc.ToString(value, noParamOrder, fallback, fallbackOnNull);

        public string ForCode(object value) => _code.Value.ForCode(value);
        public string ForCode(object value, string noParamOrder = Protector, string fallback = default) => _code.Value.ForCode(value, noParamOrder, fallback);
        

        public IJsonService Json => _jsonLazy.Value;
        private readonly LazySvc<IJsonService> _jsonLazy;

        #region Invisible Converts for backward compatibility

        public int ToInt32(object value) => ToInt(value);

        public float ToSingle(object value) => ToFloat(value);

        #endregion
    }
}
