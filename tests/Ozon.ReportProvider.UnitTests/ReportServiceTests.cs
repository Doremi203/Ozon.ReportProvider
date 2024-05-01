using System.Text.Json;
using AutoBogus;
using FluentAssertions;
using Mapster;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Ozon.ReportProvider.Bll.Services;
using Ozon.ReportProvider.Domain.Entities;
using Ozon.ReportProvider.Domain.Events;
using Ozon.ReportProvider.Domain.Exceptions;
using Ozon.ReportProvider.Domain.Interfaces.Repositories;
using Ozon.ReportProvider.Domain.Interfaces.Services;
using Ozon.ReportProvider.Domain.Models;
using Ozon.ReportProvider.Domain.ValueTypes;
using Ozon.ReportProvider.UnitTests.Config;

namespace Ozon.ReportProvider.UnitTests;

public class ReportServiceTests
{
    private readonly Mock<IReportRepository> _reportRepositoryFake;
    private readonly Mock<IDistributedCache> _distributedCacheFake;
    private readonly IReportService _reportService;

    public ReportServiceTests()
    {
        _reportRepositoryFake = new(MockBehavior.Strict);
        _distributedCacheFake = new();
        _reportService = new ReportService(_reportRepositoryFake.Object, _distributedCacheFake.Object);
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
    public async Task GetReport_CacheEmpty_ShouldCallRepository()
    {
        // Arrange
        var reportEntity = new AutoFaker<ReportEntityV1>()
            .Generate();
        var requestId = new RequestId(reportEntity.RequestId);
        var expectedReport = reportEntity.Adapt<Report>();

        _reportRepositoryFake
            .Setup(x => x.GetReport(It.IsAny<RequestId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(reportEntity);
        _distributedCacheFake
            .Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[])null!);

        // Act
        var result = await _reportService.GetReport(requestId, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedReport);

        _reportRepositoryFake.Verify(x =>
                x.GetReport(It.Is<RequestId>(r =>
                        r.Value == requestId.Value),
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetReport_CacheNotEmpty_ShouldReturnCachedReport()
    {
        // Arrange
        var reportEntity = new AutoFaker<ReportEntityV1>()
            .Generate();
        var requestId = new RequestId(reportEntity.RequestId);
        var expectedReport = reportEntity.Adapt<Report>();

        _reportRepositoryFake
            .Setup(x => x.GetReport(It.IsAny<RequestId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(reportEntity);
        _distributedCacheFake
            .Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(JsonSerializer.SerializeToUtf8Bytes(expectedReport));

        // Act
        var result = await _reportService.GetReport(requestId, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedReport);

        _reportRepositoryFake.Verify(x =>
                x.GetReport(It.IsAny<RequestId>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _distributedCacheFake.Verify(x =>
                x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetReport_CacheEmpty_ShouldCacheReport()
    {
        // Arrange
        var reportEntity = new AutoFaker<ReportEntityV1>()
            .Generate();
        var requestId = new RequestId(reportEntity.RequestId);
        var expectedReport = reportEntity.Adapt<Report>();
        var cachedReport = JsonSerializer.SerializeToUtf8Bytes(expectedReport);

        _reportRepositoryFake
            .Setup(x => x.GetReport(It.IsAny<RequestId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(reportEntity);
        _distributedCacheFake
            .Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[])null!);

        // Act
        var result = await _reportService.GetReport(requestId, It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeEquivalentTo(expectedReport);

        _reportRepositoryFake.Verify(x =>
                x.GetReport(It.Is<RequestId>(r =>
                        r.Value == requestId.Value),
                    It.IsAny<CancellationToken>()),
            Times.Once);
        _distributedCacheFake.Verify(x =>
                x.SetAsync(
                    It.IsAny<string>(),
                    It.Is<byte[]>(b =>
                        b.SequenceEqual(cachedReport)),
                    It.IsAny<DistributedCacheEntryOptions>(),
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task GetReport_NoReport_ShouldThrowReportNotReadyException()
    {
        // Arrange
        var reportEntity = new AutoFaker<ReportEntityV1>()
            .Generate();
        var requestId = new RequestId(reportEntity.RequestId);

        _reportRepositoryFake
            .Setup(x => x.GetReport(It.IsAny<RequestId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ReportEntityV1)null!);
        _distributedCacheFake
            .Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[])null!);

        // Act
        Func<Task> act = async () => await _reportService.GetReport(requestId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ReportNotReadyException>();
    }

    [Fact]
    public async Task GetUncompleteReportRequests_Success()
    {
        // Arrange
        var requests = new AutoFaker<ReportRequestEvent>().Generate(5).ToArray();
        var ids = requests.Select(r => r.RequestId).ToArray();
        var expected = requests.Skip(2).ToArray();
        _reportRepositoryFake
            .Setup(x => x.GetCompletedRequestIds(It.IsAny<RequestId[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ids.Take(2).ToArray);
        
        // Act
        var actual = await _reportService.GetUncompleteReportRequests(requests, CancellationToken.None);
        
        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
}