namespace MyLab.HttpMetrics
{
    internal interface IHttpMetricReporter
    {
        void Register();
        void RegisterUnhandledException();
    }
}