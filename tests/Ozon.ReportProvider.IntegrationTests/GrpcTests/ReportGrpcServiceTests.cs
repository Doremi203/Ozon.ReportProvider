using FluentAssertions;
using Grpc.Core;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Ozon.ReportProvider.Api;
using Ozon.ReportProvider.Domain.Entities;
using Ozon.ReportProvider.Domain.Interfaces.Repositories;
using Ozon.ReportProvider.Domain.Models;
using Ozon.ReportProvider.Domain.ValueTypes;
using Ozon.ReportProvider.IntegrationTests.Fakers;
using Ozon.ReportProvider.IntegrationTests.GrpcHelpers;
using Ozon.ReportProvider.Proto;
using Xunit.Abstractions;

namespace Ozon.ReportProvider.IntegrationTests.GrpcTests;

public class ReportGrpcServiceTests : GrpcIntegrationTestBase
{
    private readonly Mock<IReportRepository> _reportRepositoryFake = new(MockBehavior.Strict);

    public ReportGrpcServiceTests(
        GrpcTestFixture<Startup> fixture,
        ITestOutputHelper outputHelper
    ) : base(fixture, outputHelper)
    {
        Api.Config.MapsterConfig.ConfigureApiMapping();
        Bll.Config.MapsterConfig.ConfigureDomainMapping();
        fixture.ConfigureWebHost(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.Replace(new ServiceDescriptor(typeof(IReportRepository),
                        _reportRepositoryFake.Object));
                });
            }
        );
    }

    [Fact]
    public async Task GetReportV1_ReportReadyNotCached_ShouldReturnReportFromDataBase()
    {
        // Arrange
        var report = ReportEntityV1Faker.Generate()[0];
        var request = new GetReportRequestV1
        {
            RequestId = new RequestIdV1
            {
                Value = report.RequestId
            }
        };
        var expectedResponse = new GetReportResponseV1
        {
            Report = report.Adapt<Report>().Adapt<ReportV1>()
        };
        _reportRepositoryFake
            .Setup(x =>
                x.GetReport(new RequestId(report.RequestId),
                    It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(report);

        // Act
        var client = new ReportService.ReportServiceClient(Channel);
        var response = await client.GetReportV1Async(request);

        // Assert
        response.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task GetReportV1_MultipleCalls_ShouldUseCacheAfterFirstCall()
    {
        // Arrange
        var report = ReportEntityV1Faker.Generate()[0];
        var request = new GetReportRequestV1
        {
            RequestId = new RequestIdV1
            {
                Value = report.RequestId
            }
        };
        var expectedResponse = new GetReportResponseV1
        {
            Report = report.Adapt<Report>().Adapt<ReportV1>()
        };
        _reportRepositoryFake
            .Setup(x =>
                x.GetReport(new RequestId(report.RequestId),
                    It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(report);

        // Act
        var client = new ReportService.ReportServiceClient(Channel);
        var response = await client.GetReportV1Async(request);
        var response2 = await client.GetReportV1Async(request);

        // Assert
        response.Should().BeEquivalentTo(expectedResponse);
        response2.Should().BeEquivalentTo(expectedResponse);

        _reportRepositoryFake.Verify(x => x.GetReport(It.IsAny<RequestId>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetReportV1_ReportNotReady_ShouldThrowRpcExceptionUnavailable()
    {
        // Arrange
        var report = ReportEntityV1Faker.Generate()[0];
        var request = new GetReportRequestV1
        {
            RequestId = new RequestIdV1
            {
                Value = report.RequestId
            }
        };
        _reportRepositoryFake
            .Setup(x =>
                x.GetReport(new RequestId(report.RequestId),
                    It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((ReportEntityV1)null!);
        var expectedStatus = StatusCode.Unavailable;
        var errorEncountered = false;

        // Act
        var client = new ReportService.ReportServiceClient(Channel);
        try
        {
            await client.GetReportV1Async(request);
        }
        catch (RpcException e) when (e.StatusCode == expectedStatus)
        {
            errorEncountered = true;
        }
        
        // Assert
        errorEncountered.Should().BeTrue();
    }
}