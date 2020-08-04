using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MyLab.LogDsl;
using Prometheus;

namespace MyLab.HttpMetrics
{
    class HttpMetricsMiddleware
    {
        private readonly RequestDelegate _request;
        private readonly DslLogger _logger;

        public HttpMetricsMiddleware(RequestDelegate request, ILogger<HttpMetricsMiddleware> logger)
        {
            _request = request ?? throw new ArgumentNullException(nameof(request));
            _logger = logger.Dsl();
        }

        public async Task Invoke(HttpContext httpContext, HttpMetricReporterFactory reporterFactory)
        {
            var path = httpContext.Request.Path.Value;
            if (path == "/metrics")
            {
                await _request.Invoke(httpContext);
                return;
            }

            var responseContentStreamWrapper = new ContentStreamWrapper(httpContext.Response.Body);
            httpContext.Response.Body = responseContentStreamWrapper;

            IHttpMetricReporter metricReporter = null;

            var sw = Stopwatch.StartNew();

            try
            {
                await _request.Invoke(httpContext);

                sw.Stop();

                (metricReporter = CreateReporter()).Register();
            }
            catch (Exception)
            {
                (metricReporter ?? CreateReporter()).RegisterUnhandledException();
            }

            IHttpMetricReporter CreateReporter() =>
                reporterFactory.CreateReporter(
                    MetricMethodRequest.CreateFromHttpContext(httpContext),
                    MetricMethodResponse.CreateFromHttpContext(httpContext, sw.Elapsed, responseContentStreamWrapper.WriteCounter));
        }
    }
}