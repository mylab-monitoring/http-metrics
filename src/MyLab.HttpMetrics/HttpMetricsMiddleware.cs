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

        public async Task Invoke(HttpContext httpContext, IHttpMetricReporter reporter)
        {
            var path = httpContext.Request.Path.Value;
            if (path == "/metrics")
            {
                await _request.Invoke(httpContext);
                return;
            }

            var sw = Stopwatch.StartNew();

            try
            {
                await _request.Invoke(httpContext);
            }
            finally
            {
                sw.Stop();

                try
                {
                    reporter.Register(
                        MetricMethodRequest.CreateFromHttpContext(httpContext),
                        MetricMethodResponse.CreateFromHttpContext(httpContext, sw.Elapsed)
                    );
                }
                catch (Exception e)
                {
                    _logger.Error("Metric calculation error", e).Write();
                    reporter.RegisterError();
                }
            }
        }
    }
}