namespace MyLab.HttpMetrics
{
    internal interface IHttpMetricReporter
    {
        void Register(MetricMethodRequest methodRequest, MetricMethodResponse resp);
        void RegisterError();
    }
}