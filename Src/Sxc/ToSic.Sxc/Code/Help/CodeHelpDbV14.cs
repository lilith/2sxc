﻿using System.Collections.Generic;
using ToSic.Eav.Code.Help;
using static ToSic.Sxc.Code.Help.CodeHelpDbV12;
using static ToSic.Sxc.Code.Help.ObsoleteHelp;

namespace ToSic.Sxc.Code.Help
{
    internal class CodeHelpDbV14
    {
        internal static CodeHelp HelpRemoved14(string property, string linkCode, params (string Code, string Comment)[] alt)
            => HelpNotExists((property, null), "was a bad design choice in Razor14 and had to be removed - see link", linkCode, alt);

        internal static CodeHelp AsTypedRemoved14 = HelpRemoved14("AsTyped", "brc-1602", null);

        internal static CodeHelp AsTypedListRemoved14 = HelpRemoved14("AsTypedList", "brc-1602", null);

        internal static List<CodeHelp> Compile14 = new List<CodeHelp>
        {
            // use `Convert`
            SystemConvertIncorrectUse,

            // Use Dnn
            DnnObjectNotInHybrid,

            // use `CreateSource(name)
            CreateSourceStringObsolete,

            // Not handled - can't because the AsDynamic accepts IEntity which works in Razor14
            // dynamic AsDynamic(ToSic.Eav.Interfaces.IEntity entity)
            // dynamic AsDynamic(KeyValuePair<int, ToSic.Eav.Interfaces.IEntity> entityKeyValuePair)
            // IEnumerable<dynamic> AsDynamic(IEnumerable<ToSic.Eav.Interfaces.IEntity> entities)
            // dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair) => Obsolete10.AsDynamicKvp();

            // Access .List
            ListNotExist12,

            ListObsolete12,
            ListObsolete12MisCompiledAsGenericList,

            // Access ListContent
            ListContentNotExist12,
            ListPresentationNotExist12,

            // Presentation
            PresentationNotExist12,

            // Skipped, as can't be detected - they are all IEnumerable...
            //[PrivateApi] public IEnumerable<dynamic> AsDynamic(IDataStream stream) => Obsolete10.AsDynamicForList();
            //[PrivateApi] public IEnumerable<dynamic> AsDynamic(IDataSource source) => Obsolete10.AsDynamicForList();
            //[PrivateApi] public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities) => Obsolete10.AsDynamicForList();

            // v16.01 AsTyped / AsTypedList
            AsTypedRemoved14,
            AsTypedListRemoved14,
        };
    }
}
