using System.Reflection.Metadata;
using Prometheus;

namespace MyLab.HttpMetrics
{
    class HttpMetricReporter
    {
        private readonly Counter _requestCounter;
        private readonly Histogram _requestProcTimeHistogram;
        private readonly Histogram _requestContentSizeHistogram;
        private readonly Histogram _responseContentSizeHistogram;
        private readonly Counter _errorCounter;
        private Counter _requestSizeCounter;
        private Counter _responseSizeCounter;

        /// <summary>
        /// Initializes a new instance of <see cref="HttpMetricReporter"/>
        /// </summary>
        public HttpMetricReporter()
        {
            var labels = new[]
            {
                HttpMetricConstants.HttpMethodLabel,
                HttpMetricConstants.HttpPathLabel,
                HttpMetricConstants.HttpStatusCodeLabel,
            };

            _requestCounter = Metrics.CreateCounter(
                HttpMetricConstants.RequestReceivedMetricName,
                "The total number of requests",
                labels);

            _errorCounter = Metrics.CreateCounter(
                HttpMetricConstants.MetricCollectingErrorCount,
                "The total number of metric collecting errors",
                labels);

            _requestSizeCounter = Metrics.CreateCounter(
                HttpMetricConstants.RequestContentSizeCounter,
                "The total size of request content",
                labels);

            _responseSizeCounter = Metrics.CreateCounter(
                HttpMetricConstants.ResponseContentSizeCounter,
                "The total size of response content",
                labels);

            _requestProcTimeHistogram = Metrics.CreateHistogram(
                HttpMetricConstants.RequestDurationMetricName,
                "The duration in seconds between the response to a request.", 
                new HistogramConfiguration
                {
                    Buckets = Histogram.ExponentialBuckets(0.01, 2, 13),
                    LabelNames = labels
                });

            _requestContentSizeHistogram = Metrics.CreateHistogram(
                HttpMetricConstants.RequestContentSizeMetricName,
                "The request content size in bytes.",
                new HistogramConfiguration
                {
                    Buckets = Histogram.ExponentialBuckets(1024, 2, 11),
                    LabelNames = labels
                });

            _responseContentSizeHistogram = Metrics.CreateHistogram(
                HttpMetricConstants.ResponseContentSizeMetricName,
                "The response content size in bytes.",
                new HistogramConfiguration
                {
                    Buckets = Histogram.ExponentialBuckets(1024, 2, 11),
                    LabelNames = labels
                });
        }

        public void Register(MetricMethodRequest methodRequest, MetricMethodResponse resp)
        {
            var labels = new[] {methodRequest.HttpMethod, methodRequest.UrlPath, resp.ResponseCode};

            _requestCounter.Labels(labels).Inc();
            _requestCounter.Labels(labels).Inc();

            if(methodRequest.Length.HasValue)
                _requestSizeCounter.Labels(labels).Inc(methodRequest.Length.Value);

            if(resp.Length.HasValue)
                _responseSizeCounter.Labels(labels).Inc(resp.Length.Value);

            _requestProcTimeHistogram.Labels(labels).Observe(resp.ElapsedTime.TotalSeconds);
            _requestContentSizeHistogram.Labels(labels).Observe(methodRequest.Length ?? 0);
            _responseContentSizeHistogram.Labels(labels).Observe(resp.Length ?? 0);
        }

        public void RegisterError()
        {
            _errorCounter.Inc();
        }
    }
}