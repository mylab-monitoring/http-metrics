using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace MyLab.HttpMetrics
{
    /// <summary>
    /// Contains extension methods to http metrics integration
    /// </summary>
    public static class HttpMetricsIntegration
    {

        public static IServiceCollection AddUrlBasedHttpMetrics(this IServiceCollection srv)
        {
            if (srv == null) throw new ArgumentNullException(nameof(srv));

            return srv
                .AddSingleton<HttpMetricReporterFactory>();
        }

        public static IApplicationBuilder UseUrlBasedHttpMetrics(this IApplicationBuilder appBuilder)
        {
            if (appBuilder == null) throw new ArgumentNullException(nameof(appBuilder));

            return appBuilder.UseMiddleware<HttpMetricsMiddleware>();
        }
    }
}
