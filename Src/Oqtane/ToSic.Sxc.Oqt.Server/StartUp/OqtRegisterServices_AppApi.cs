﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Code;
using ToSic.Sxc.Oqt.Server.Code;
using ToSic.Sxc.Oqt.Server.Controllers.AppApi;

namespace ToSic.Sxc.Oqt.Server.StartUp
{
    // ReSharper disable once InconsistentNaming
    internal static partial class OqtRegisterServices
    {
        public static IServiceCollection AddOqtAppWebApi(this IServiceCollection services)
        {
            // App WebApi: .net specific code compiler
            services.TryAddTransient<CodeCompiler, CodeCompilerNetCore>();

            services.AddSingleton<IActionDescriptorChangeProvider>(AppApiActionDescriptorChangeProvider.Instance);
            services.AddSingleton(AppApiActionDescriptorChangeProvider.Instance);
            services.AddSingleton<AppApiFileSystemWatcher>();
            services.AddScoped<AppApiDynamicRouteValueTransformer>();
            services.AddScoped<AppApiControllerManager>();
            services.AddScoped<AppApiActionContext>();
            services.AddScoped<AppApiAuthorization>();
            services.AddScoped<AppApiActionInvoker>();
            services.AddScoped<IAuthorizationHandler, AppApiPermissionHandler>();

            return services;
        }
    }
}