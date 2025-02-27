﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CSharp.RuntimeBinder;
using ToSic.Eav;
using ToSic.Eav.Code.Help;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
#if NETFRAMEWORK
using HttpCompileException = System.Web.HttpCompileException;
#else
// TODO: What's the real compile exception type?
using HttpCompileException = System.Exception;
#endif

namespace ToSic.Sxc.Code.Help
{
    public class CodeErrorHelpService: ServiceBase
    {
        public CodeErrorHelpService() : base("Sxc.CErrHS")
        {
            Log.A("Trying to add help to error, something must have happened");
        }

        public Exception AddHelpForCompileProblems(Exception ex, CodeFileTypes fileType)
        {
            var l = Log.Fn<Exception>();
            try
            {
                // Check if it already has help included
                if (ex is IExceptionWithHelp) 
                    return l.Return(ex, "already has help");

                if (!CodeHelpDb.CompileHelp.TryGetValue(fileType, out var list))
                    return l.Return(ex, "no additional help found");

                var help = FindManyOrNull(ex, list);
                return help == null 
                    ? l.Return(ex)
                    : l.Return(new ExceptionWithHelp(help, ex), "added help");
            }
            catch (Exception myEx)
            {
                Log.Ex("Something went wrong, inner error", myEx);
                return l.Return(ex, "just return original exception");
            }
        }

        public Exception AddHelpIfKnownError(Exception ex, object mainCodeObject)
        {
            var l = Log.Fn<Exception>();
            try
            {
                // Check if it already has help included
                if (ex is IExceptionWithHelp)
                    return l.Return(ex, "already has help");

                var help = FindHelp(ex);
                if (help != null)
                    return l.Return(new ExceptionWithHelp(help, ex), "added help");

                if (mainCodeObject is IHasCodeHelp withHelp && withHelp.ErrorHelpers.SafeAny())
                    help = FindHelp(ex, withHelp.ErrorHelpers);

                return help == null
                    ? l.Return(ex)
                    : l.Return(new ExceptionWithHelp(help, ex), "added help");
            }
            catch (Exception myEx)
            {
                Log.Ex("Something went wrong, inner error", myEx);
                return l.Return(ex, "just return original exception");
            }

        }

        internal CodeHelp FindHelp(Exception ex)
        {
            switch (ex)
            {
                // Check if we already wrapped it
                case ExceptionWithHelp _:
                    return null;
                //case NamedArgumentException nae:
                //    return new CodeHelp("named-parameters", null,
                //        Parameters.HelpLink,
                //        uiMessage: " ", detailsHtml: nae.Intro.Replace("\n", "<br>") + (nae.ParamNames.HasValue() ? $"<br>Param Names: <code>{nae.ParamNames}</code>": ""));
                case RuntimeBinderException _:
                    return FindHelp(ex, CodeHelpList.ListRuntime);
                case InvalidCastException _:
                    return FindHelp(ex, CodeHelpList.ListInvalidCast);
                case HttpCompileException _:
                    return FindHelp(ex, CodeHelpList.ListHttpCompile);
                default:
                    return null;
            }
        }

        private static CodeHelp FindHelp(Exception ex, List<CodeHelp> errorList)
        {
            var msg = ex?.Message;
            return msg == null ? null : errorList.FirstOrDefault(help => help.DetectRegex ? Regex.IsMatch(msg, help.Detect) : msg.Contains(help.Detect));
        }
        private static List<CodeHelp> FindManyOrNull(Exception ex, List<CodeHelp> errorList)
        {
            var msg = ex?.Message;
            if (msg.IsEmptyOrWs()) return null;
            var list = errorList
                .Where(help => help.DetectRegex ? Regex.IsMatch(msg, help.Detect) : msg.Contains(help.Detect))
                .ToList();
            return list.Any() ? list : null;
        }

    }
}
