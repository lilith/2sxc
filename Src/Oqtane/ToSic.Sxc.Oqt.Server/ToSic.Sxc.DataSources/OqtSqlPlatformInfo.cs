﻿using System;
using Oqtane.Infrastructure;
using Oqtane.Shared;
using ToSic.Eav.DataSources;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;

namespace ToSic.Sxc.Oqt.Server.ToSic.Sxc.DataSources
{
    public class OqtSqlPlatformInfo: SqlPlatformInfo
    {
        private readonly LazySvc<IConfigManager> _configManager;
        public override string DefaultConnectionStringName => SettingKeys.ConnectionStringKey;

        public OqtSqlPlatformInfo(LazySvc<IConfigManager> configManager)
        {
            _configManager = configManager;
        }

        public override string FindConnectionString(string name)
        {
            if (name.EqualsInsensitive(DefaultConnectionStringName))
                return _configManager.Value.GetSetting("ConnectionStrings:" + SettingKeys.ConnectionStringKey, "");

            // TODO
            // Where are all the connection strings stored, I think base... doesn't work
            // Where would the site connection string be?
            return base.FindConnectionString(name);
        }

    }
}
