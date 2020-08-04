using System;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using MyLab.ApiClient;
using MyLab.ApiClient.Test;
using MyLab.HttpMetrics;
using Prometheus;
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

        [Theory]
        [MemberData(nameof(CreateMetricNames))]
        public async Task ShouldExposeRequestCount(string expectedMetricName, string expectedLe)
        {
            //Arrange
            var le = expectedLe != null ? $",le=\"{expectedLe}\"" : "";
            var expectedMetric = $"{expectedMetricName}{{method=\"POST\",path=\"/api/test/post/xxx/data\",status_code=\"200\"{le}}}";
            await TestCall(srv => srv.Post(0, "data"));

            //Act
            var metrics = await TestCall(srv => srv.GetMetrics());

            //Assert
            Assert.Contains(expectedMetric, metrics.ResponseContent);
        }

        [Fact]
        public async Task ShouldExposeUnhandledExceptionMetric()
        {
            //Arrange
            await TestCall(srv => srv.GetException());

            //Act
            var metrics = await TestCall(srv => srv.GetMetrics());

            //Assert
            Assert.Contains("ml_http_unhandled_exception_total{method=\"GET\",path=\"/api/test/get/exception\",status_code=\"200\"}", metrics.ResponseContent);
        }

        public static object[][]CreateMetricNames()
        {
            return new []
            {
                new object[]{"ml_http_request_count_total", null},
                new object[]{"ml_http_request_size_bytes_total", null},
                new object[]{"ml_http_response_size_bytes_total", null},

                new object[]{"ml_http_request_content_size_bytes_sum", null},
                new object[]{"ml_http_request_content_size_bytes_count", null},
                new object[]{"ml_http_request_content_size_bytes_bucket", "1024"},
                new object[]{"ml_http_request_content_size_bytes_bucket", "2048"},
                new object[]{"ml_http_request_content_size_bytes_bucket", "5120"},
                new object[]{"ml_http_request_content_size_bytes_bucket", "10240"},
                new object[]{"ml_http_request_content_size_bytes_bucket", "20480"},
                new object[]{"ml_http_request_content_size_bytes_bucket", "51200"},
                new object[]{"ml_http_request_content_size_bytes_bucket", "1048576"},
                new object[]{"ml_http_request_content_size_bytes_bucket", "+Inf"},

                new object[]{"ml_http_response_content_size_bytes_sum", null},
                new object[]{"ml_http_response_content_size_bytes_count", null},
                new object[]{"ml_http_response_content_size_bytes_bucket", "1024"},
                new object[]{"ml_http_response_content_size_bytes_bucket", "2048"},
                new object[]{"ml_http_response_content_size_bytes_bucket", "5120"},
                new object[]{"ml_http_response_content_size_bytes_bucket", "10240"},
                new object[]{"ml_http_response_content_size_bytes_bucket", "20480"},
                new object[]{"ml_http_response_content_size_bytes_bucket", "51200"},
                new object[]{"ml_http_response_content_size_bytes_bucket", "1048576"},
                new object[]{"ml_http_response_content_size_bytes_bucket", "+Inf"},

                new object[]{"ml_http_request_duration_seconds_sum", null},
                new object[]{"ml_http_request_duration_seconds_count", null},

                new object[]{"ml_http_request_duration_seconds_bucket", "0.01"},
                new object[]{"ml_http_request_duration_seconds_bucket", "0.02"},
                new object[]{"ml_http_request_duration_seconds_bucket", "0.05"},
                new object[]{"ml_http_request_duration_seconds_bucket", "0.1"},
                new object[]{"ml_http_request_duration_seconds_bucket", "0.2"},
                new object[]{"ml_http_request_duration_seconds_bucket", "0.5"},
                new object[]{"ml_http_request_duration_seconds_bucket", "1"},
                new object[]{"ml_http_request_duration_seconds_bucket", "2"},
                new object[]{"ml_http_request_duration_seconds_bucket", "5"},
                new object[]{"ml_http_request_duration_seconds_bucket", "10"},
                new object[]{"ml_http_request_duration_seconds_bucket", "20"},
                new object[]{"ml_http_request_duration_seconds_bucket", "30"},
                new object[]{"ml_http_request_duration_seconds_bucket", "+Inf"},
            };
        }
    }

    [Api("")]
    public interface ITestService
    {
        [Post("api/test/post/{id}/data")]
        Task Post([Path]int id, [JsonContent]string data);

        [Get("api/test/get/exception")]
        Task GetException();

        [Get("metrics")]
        Task<string> GetMetrics();
    }
}
