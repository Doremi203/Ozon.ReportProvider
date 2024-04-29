using AutoBogus;
using Mapster;
using Moq;
using Ozon.ReportProvider.Bll.Services;
using Ozon.ReportProvider.Domain.Events;
using Ozon.ReportProvider.Domain.Interfaces.Services;
using Ozon.ReportProvider.Domain.Models;

namespace Ozon.ReportProvider.UnitTests;

public class ReportRequestServiceTests
{
    private readonly Mock<IApiReportService> _apiReportServiceFake;
    private readonly Mock<IReportService> _reportServiceFake;
    private readonly IReportRequestService _reportRequestService;

    public ReportRequestServiceTests()
    {
        _apiReportServiceFake = new(MockBehavior.Strict);
        _reportServiceFake = new(MockBehavior.Strict);
        _reportRequestService = new ReportRequestService(
            _apiReportServiceFake.Object, _reportServiceFake.Object);
    }

    [Fact]
    public async Task ProcessReportRequests_Success()
    {
        // Arrange
        var reportRequestEvents = new AutoFaker<ReportRequestEvent>()
            .Generate(3).ToArray();
        var getReportModels = reportRequestEvents.Adapt<ApiGetReportModel[]>();
        var reports = new AutoFaker<Report>()
            .Generate(3).ToArray();

        _apiReportServiceFake
            .Setup(x => x.GetReports(It.IsAny<ApiGetReportModel[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(reports);
        _reportServiceFake
            .Setup(x => x.StoreReports(It.IsAny<Report[]>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _reportRequestService.ProcessReportRequests(reportRequestEvents, CancellationToken.None);

        // Assert
        _apiReportServiceFake.Verify(x =>
                x.GetReports(It.Is<ApiGetReportModel[]>(m =>
                        m.SequenceEqual(getReportModels)),
                    It.IsAny<CancellationToken>()),
            Times.Once);
        _reportServiceFake.Verify(x =>
                x.StoreReports(It.Is<Report[]>(r =>
                        r.SequenceEqual(reports)),
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }
}