using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Ozon.ReportProvider.Api;
using Xunit.Abstractions;

namespace Ozon.ReportProvider.IntegrationTests.GrpcHelpers;

public class GrpcIntegrationTestBase : IClassFixture<GrpcTestFixture<Startup>>, IDisposable
{
    private IDisposable? _testContext;

    protected GrpcTestFixture<Startup> Fixture { get; set; }

    protected ILoggerFactory LoggerFactory => Fixture.LoggerFactory;

    protected GrpcChannel Channel => CreateChannel();

    protected GrpcChannel CreateChannel()
    {
        return GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions
        {
            LoggerFactory = LoggerFactory,
            HttpHandler = Fixture.Handler
        });
    }

    public GrpcIntegrationTestBase(GrpcTestFixture<Startup> fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        _testContext = Fixture.GetTestContext(outputHelper);
    }

    public void Dispose()
    {
        _testContext?.Dispose();
    }
}