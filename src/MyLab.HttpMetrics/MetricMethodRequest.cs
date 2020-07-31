using System;
using Microsoft.AspNetCore.Http;

namespace MyLab.HttpMetrics
{
    class MetricMethodRequest
    {
        public string UrlPath { get; set; }
        public string HttpMethod { get; set; }
        public long? Length { get; set; }

        public static MetricMethodRequest CreateFromHttpContext(HttpContext ctx)
        {
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));

            var normPath = UrlPathNormalizer.Normalize(ctx.Request.Path.Value);

            return new MetricMethodRequest
            {
                HttpMethod = ctx.Request.Method,
                UrlPath = normPath,
                Length = ctx.Request.ContentLength
            };
        }
    }
}