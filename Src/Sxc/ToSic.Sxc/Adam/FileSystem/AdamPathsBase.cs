﻿using System;
using System.IO;
using ToSic.Eav.Helpers;
using ToSic.Lib.Logging;
using ToSic.Eav.Run;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Adam
{
    public class AdamPathsBase : ServiceBase, IAdamPaths
    {
        #region DI Constructor & Init

        public AdamPathsBase(IServerPaths serverPaths) : this(serverPaths, LogScopes.Base)
        {

        }

        protected AdamPathsBase(IServerPaths serverPaths, string logPrefix) : base($"{logPrefix}.AdmPth")
        {
            ConnectServices(
                _serverPaths = serverPaths
            );
        }
        private readonly IServerPaths _serverPaths;

        public IAdamPaths Init(AdamManager adamManager)
        {
            AdamManager = adamManager;
            return this;
        }
        protected AdamManager AdamManager { get; private set; }

        #endregion



        public string PhysicalPath(string path)
        {
            ThrowIfPathContainsDotDot(path);

            // check if it's already a physical path
            if (Path.IsPathRooted(path)) return path;

            // check if it already has the root path attached, otherwise add
            path = path.StartsWith(AdamManager.Site.ContentPath) ? path : Path.Combine(AdamManager.Site.ContentPath, path);
            return _serverPaths.FullContentPath(path.Backslash());
        }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        public static void ThrowIfPathContainsDotDot(string path)
        {
            if (path.Contains("..")) throw new ArgumentException("path may not contain ..", nameof(path));
        }

        public string RelativeFromAdam(string path)
        {
            var adamPosition = path.ForwardSlash().IndexOf("adam/", StringComparison.InvariantCultureIgnoreCase);
            return adamPosition <= 0
                ? path
                : path.Substring(adamPosition);
        }

        /// <summary>
        /// Will receive the path as is on the file system, and return the url form how it would be called from outside.
        /// This default implementation assumes the path of the server and url are the same.
        /// In .net core this will be different, so it must replace the internal logic
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public virtual string Url(string path) => Path.Combine(AdamManager.Site.ContentPath, path).ForwardSlash();
    }
}
