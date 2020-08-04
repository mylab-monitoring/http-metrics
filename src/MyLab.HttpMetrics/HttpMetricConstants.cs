using System;

namespace MyLab.HttpMetrics
{
    static class HttpMetricConstants
    {
        public static readonly string RequestContentSizeCounter= "ml_http_request_size_bytes_total";
        public static readonly string ResponseContentSizeCounter = "ml_http_response_size_bytes_total";
        public static readonly string UnhandledExceptionCount = "ml_http_unhandled_Exception_count";
        public static readonly string RequestDurationMetricName = "ml_http_request_duration_seconds";
        public static readonly string RequestContentSizeMetricName = "ml_http_request_content_size_bytes";
        public static readonly string ResponseContentSizeMetricName = "ml_http_response_content_size_bytes";
        public static readonly string RequestReceivedMetricName = "ml_http_request_count_total";
        public static readonly string HttpMethodLabel = "method";
        public static readonly string HttpPathLabel = "path";
        public static readonly string HttpStatusCodeLabel = "status_code";
    }
}
