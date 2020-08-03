using System;
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
        private readonly Counter _requestSizeCounter;
        private readonly Counter _responseSizeCounter;

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
                    Buckets = new []
                    {
                        0.01,   //10ms
                        0.02,   //20ms
                        0.05,   //50ms
                        0.1,    //100ms
                        0.2,    //200ms
                        0.5,    //500ms
                        1,      //1s
                        2,      //2s
                        5,      //3s
                        10,     //10s
                        20,     //20s
                        30      //301s
                    }, 
                    LabelNames = labels
                });

            var sizeBuckets = new Double[]
            {
                1024,           //1kb
                1024*2,         //2kb
                1024*5,         //5kb
                1024*10,        //10kb
                1024*10*2,      //20kb
                1024*10*5,      //30kb
                1024*1024       //1Mb
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

        public void Register(MetricMethodRequest methodRequest, MetricMethodResponse resp)
        {
            var labels = new[] {methodRequest.HttpMethod, methodRequest.UrlPath, resp.ResponseCode};

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