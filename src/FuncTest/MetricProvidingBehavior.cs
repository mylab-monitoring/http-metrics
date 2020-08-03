using System;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using MyLab.ApiClient;
using MyLab.ApiClient.Test;
using MyLab.HttpMetrics;
using TestServer;
using Xunit;
using Xunit.Abstractions;

namespace FuncTest
{
    public class MetricProvidingBehavior : ApiClientTest<Startup, ITestService>
    {
        public MetricProvidingBehavior(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldExposeRequestCount()
        {
            //Arrange
            var reqCounterName = CreateMetricName(
                HttpMetricConstants.RequestReceivedMetricName, 
                "GET", 
                "/api/test/get/xxx/data",
                "200");
            await TestCall(srv => srv.Get(0));

            //Act
            var metrics = await TestCall(srv => srv.GetMetrics());

            //Assert
            Assert.Contains($"{reqCounterName} 1", metrics.ResponseContent);
        }

        string CreateMetricName(string name, string method, string path, string respCode)
        {
            return $"{name}{{{HttpMetricConstants.HttpMethodLabel}=\"{method}\",{HttpMetricConstants.HttpPathLabel}=\"{path}\",{HttpMetricConstants.HttpStatusCodeLabel}=\"{respCode}\"}}";
        }
    }

    [Api("")]
    public interface ITestService
    {
        [Get("api/test/get/{id}/data")]
        Task Get([Path]int id);

        [Post("api/test/post/{id}/data")]
        Task Post([Path]int id, [JsonContent]string data);

        [Get("metrics")]
        Task<string> GetMetrics();
    }
}
