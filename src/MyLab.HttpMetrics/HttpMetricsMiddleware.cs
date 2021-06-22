using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MyLab.Log.Dsl;
using Prometheus;

namespace MyLab.HttpMetrics
{
    class HttpMetricsMiddleware
    {
        private readonly RequestDelegate _request;

        public HttpMetricsMiddleware(RequestDelegate request)
        {
            _request = request ?? throw new ArgumentNullException(nameof(request));
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

                throw;
            }

            IHttpMetricReporter CreateReporter() =>
                reporterFactory.CreateReporter(
                    MetricMethodRequest.CreateFromHttpContext(httpContext),
                    MetricMethodResponse.CreateFromHttpContext(httpContext, sw.Elapsed, responseContentStreamWrapper.WriteCounter));
        }
    }
}