using AutoBogus;
using FluentAssertions;
using Mapster;
using Moq;
using Ozon.ReportProvider.Bll.Services;
using Ozon.ReportProvider.Domain.Entities;
using Ozon.ReportProvider.Domain.Interfaces.Repositories;
using Ozon.ReportProvider.Domain.Interfaces.Services;
using Ozon.ReportProvider.Domain.Models;
using Ozon.ReportProvider.Domain.ValueTypes;
using Ozon.ReportProvider.UnitTests.Config;

namespace Ozon.ReportProvider.UnitTests;

public class ReportServiceTests
{
    private readonly Mock<IReportRepository> _reportRepositoryFake;
    private readonly IReportService _reportService;

    public ReportServiceTests()
    {
        _reportRepositoryFake = new(MockBehavior.Strict);
        _reportService = new ReportService(_reportRepositoryFake.Object);
        MapsterConfig.Configure();
    }

    [Fact]
    public async Task StoreReports_Success()
    {
        // Arrange
        var reports = new AutoFaker<Report>()
            .Generate(3).ToArray();
        var reportEntities = reports.Adapt<ReportEntityV1[]>();

        _reportRepositoryFake
            .Setup(x => x.Add(It.IsAny<ReportEntityV1[]>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _reportService.StoreReports(reports, CancellationToken.None);

        // Assert
        _reportRepositoryFake.Verify(x =>
                x.Add(It.Is<ReportEntityV1[]>(r =>
                        r.SequenceEqual(reportEntities)),
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task GetReports_Success()
    {
        // Arrange
        var reportEntities = new AutoFaker<ReportEntityV1>()
            .Generate(3).ToArray();
        var requestIds = reportEntities.Select(x => new RequestId(x.RequestId)).ToArray();

        _reportRepositoryFake
            .Setup(x => x.GetReports(It.IsAny<RequestId[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(reportEntities);

        // Act
        var result = await _reportService.GetReports(requestIds, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(reportEntities.Adapt<Report[]>());
        
        _reportRepositoryFake.Verify(x =>
                x.GetReports(It.Is<RequestId[]>(r =>
                        r.SequenceEqual(requestIds)),
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }
}