﻿using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Shared;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Oqt.Server.Services
{
    public class OqtSystemLogService : ISystemLogService
    {
        private readonly ILogManager _logManager;

        public OqtSystemLogService(ILogManager logManager)
        {
            _logManager = logManager;
        }
        
        public void Add(string title, string message)
        {
            _logManager.Log(LogLevel.Information, title, LogFunction.Other, message);
        }
    }

    // TODO: WIP, need to enhance Logging in Oqtane using LogManager, but find better method (like prefill new Log())
    // because "Title" is not stored log as expected 

    //public class SxcAppEventLogHost
    //{
    //    public SxcAppEventLogHost(string title)
    //    {
    //        Title = title;
    //    }

    //    public string Title { get; }

    //    public override string? ToString()
    //    {
    //        return Title;
    //    }
    //}
}
