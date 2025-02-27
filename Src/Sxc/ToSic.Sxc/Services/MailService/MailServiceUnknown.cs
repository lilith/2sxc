﻿using System.Net.Mail;
using ToSic.Eav.Context;
using ToSic.Eav.Run.Unknown;
using ToSic.Lib.DI;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Services
{
    internal class MailServiceUnknown : MailServiceBase
    {
        public MailServiceUnknown(WarnUseOfUnknown<MailServiceUnknown> _, LazySvc<IUser> userLazy) : base(userLazy)
        { }

        protected override SmtpClient SmtpClient()
        {
            throw new System.NotImplementedException();
        }
    }
}