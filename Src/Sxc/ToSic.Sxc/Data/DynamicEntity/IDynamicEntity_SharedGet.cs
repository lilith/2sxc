﻿using ToSic.Sxc.Data.Docs;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data
{
    public partial interface IDynamicEntity
    {
        /* IMPORTANT: KEEP THIS DEFINITION AND DOCS IN SYNC BETWEEN IDynamicEntity, IDynamicEntityBase and IDynamicStack */
        /// <inheritdoc cref=DynamicEntityDocss.Get(string)"/>
#if NETFRAMEWORK
        new
#endif
            dynamic Get(string name);


        /* IMPORTANT: KEEP THIS DEFINITION AND DOCS IN SYNC BETWEEN IDynamicEntity, IDynamicEntityBase and IDynamicStack */
        /// <inheritdoc cref="DynamicEntityDocs.Get(string, string, string, bool, bool?)"/>
            // ReSharper disable once MethodOverloadWithOptionalParameter
        dynamic Get(string name, string noParamOrder = Protector, string language = null, bool convertLinks = true, bool? debug = null);

        /// <inheritdoc cref="DynamicEntityDocs.Get{TValue}(string)"/>
        TValue Get<TValue>(string name);

        /// <inheritdoc cref="DynamicEntityDocs.Get{TValue}(string, string, TValue)"/>
        // ReSharper disable once MethodOverloadWithOptionalParameter
        TValue Get<TValue>(string name, string noParamOrder = Protector, TValue fallback = default);

    }
}
