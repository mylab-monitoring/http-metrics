using System;
using Microsoft.AspNetCore.Http;

namespace MyLab.HttpMetrics
{
    class MetricMethodResponse
    {
        public TimeSpan ElapsedTime { get; set; }
        public string ResponseCode { get; set; }
        public long? Length { get; set; }

        public static MetricMethodResponse CreateFromHttpContext(HttpContext ctx, TimeSpan elapsedTime, long length)
        {
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));

            return new MetricMethodResponse
            {
                ElapsedTime = elapsedTime,
                ResponseCode = ctx.Response.StatusCode.ToString(),
                Length = length
            };
        }
    }
}