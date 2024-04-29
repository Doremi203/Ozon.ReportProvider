using FluentAssertions;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Ozon.ReportProvider.Api;
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
    public async Task GetReportsV1_ValidRequest_ShouldReturnGetReportsResponseV1()
    {
        // Arrange
        var reports = ReportEntityV1Faker.Generate();
        var requests = new GetReportsRequestV1
        {
            RequestIds = { reports.Select(x => new RequestIdV1
            {
                Value = x.RequestId
            })}
        };
        _reportRepositoryFake
            .Setup(x => x.GetReports(It.IsAny<RequestId[]>(), default))
            .ReturnsAsync(reports);
        
        var expectedResponse = new GetReportsResponseV1
        {
            Reports = { reports.Adapt<Report[]>().Adapt<ReportV1[]>() }
        };

        // Act
        var client = new ReportsService.ReportsServiceClient(Channel);
        var response = await client.GetReportsV1Async(requests);

        // Assert
        response.Should().BeEquivalentTo(expectedResponse);
    }
}