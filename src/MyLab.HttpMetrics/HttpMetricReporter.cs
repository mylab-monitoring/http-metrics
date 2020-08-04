using Prometheus;

namespace MyLab.HttpMetrics
{
    class HttpMetricReporter : IHttpMetricReporter
    {
        private readonly MetricMethodRequest _request;
        private readonly MetricMethodResponse _response;
        private readonly string[] _labels;

    public  Counter RequestCounter { get; set; }
        public  Histogram RequestProcTimeHistogram { get; set; }
        public  Histogram RequestContentSizeHistogram { get; set; }
        public  Histogram ResponseContentSizeHistogram { get; set; }
        public  Counter RequestSizeCounter { get; set; }
        public  Counter ResponseSizeCounter { get; set; }
        public  Counter UnhandledExceptionCounter { get; set; }

        public HttpMetricReporter(MetricMethodRequest request, MetricMethodResponse response)
        {
            _request = request;
            _response = response;

            _labels = new[] {_request.HttpMethod, _request.UrlPath, _response.ResponseCode};
        }

        public void Register()
        {
            RequestCounter.Labels(_labels).Inc();

            if (_request.Length.HasValue)
                RequestSizeCounter.Labels(_labels).Inc(_request.Length.Value);

            if (_response.Length.HasValue)
                ResponseSizeCounter.Labels(_labels).Inc(_response.Length.Value);

            RequestProcTimeHistogram.Labels(_labels).Observe(_response.ElapsedTime.TotalSeconds);
            RequestContentSizeHistogram.Labels(_labels).Observe(_request.Length ?? 0);
            ResponseContentSizeHistogram.Labels(_labels).Observe(_response.Length ?? 0);
        }

        public void RegisterUnhandledException()
        {
            UnhandledExceptionCounter.Labels(_labels).Inc();
        }

    }
}