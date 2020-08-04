using System;
using Prometheus;

namespace MyLab.HttpMetrics
{
    class HttpMetricReporterFactory
    {
        private readonly Counter _requestCounter;
        private readonly Histogram _requestProcTimeHistogram;
        private readonly Histogram _requestContentSizeHistogram;
        private readonly Histogram _responseContentSizeHistogram;
        private readonly Counter _requestSizeCounter;
        private readonly Counter _responseSizeCounter;
        private readonly Counter _unhandledExceptionCounter;

        public  HttpMetricReporterFactory()
        {
            var labels = new[]
            {
                HttpMetricConstants.HttpMethodLabel,
                HttpMetricConstants.HttpPathLabel,
                HttpMetricConstants.HttpStatusCodeLabel,
            };

            _requestCounter = Metrics.CreateCounter(
                HttpMetricConstants.RequestReceivedCounter,
                "The total number of requests",
                labels);

            _unhandledExceptionCounter = Metrics.CreateCounter(
                HttpMetricConstants.UnhandledExceptionCounter,
                "The total number of unhandled exception",
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
                    Buckets = new[]
                    {
                        0.01, //10ms
                        0.02, //20ms
                        0.05, //50ms
                        0.1, //100ms
                        0.2, //200ms
                        0.5, //500ms
                        1, //1s
                        2, //2s
                        5, //3s
                        10, //10s
                        20, //20s
                        30 //30s
                    },
                    LabelNames = labels
                });

            var sizeBuckets = new Double[]
            {
                1024, //1kb
                1024 * 2, //2kb
                1024 * 5, //5kb
                1024 * 10, //10kb
                1024 * 10 * 2, //20kb
                1024 * 10 * 5, //50kb
                1024 * 1024 //1Mb
            };


            _requestContentSizeHistogram = Metrics.CreateHistogram(
                HttpMetricConstants.RequestContentSizeMetricName,
                "The request content size in bytes.",
                new HistogramConfiguration
                {
                    Buckets = sizeBuckets,
                    LabelNames = labels
                });

            _responseContentSizeHistogram = Metrics.CreateHistogram(
                HttpMetricConstants.ResponseContentSizeMetricName,
                "The response content size in bytes.",
                new HistogramConfiguration
                {
                    Buckets = sizeBuckets,
                    LabelNames = labels
                });
        }

        public IHttpMetricReporter CreateReporter(MetricMethodRequest methodRequest, MetricMethodResponse resp)
        {
            if (methodRequest == null) throw new ArgumentNullException(nameof(methodRequest));
            if (resp == null) throw new ArgumentNullException(nameof(resp));

            return new HttpMetricReporter(methodRequest, resp)
            {
                RequestContentSizeHistogram = _requestContentSizeHistogram,
                RequestCounter = _requestCounter,
                RequestProcTimeHistogram = _requestProcTimeHistogram,
                RequestSizeCounter = _requestSizeCounter,
                ResponseContentSizeHistogram = _responseContentSizeHistogram,
                ResponseSizeCounter = _responseSizeCounter,
                UnhandledExceptionCounter = _unhandledExceptionCounter
            };
        }
    }
}