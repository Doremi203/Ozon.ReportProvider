using Microsoft.Extensions.Logging;
using Ozon.ReportProvider.Domain.Interfaces.Services;
using Ozon.ReportProvider.Domain.Models;

namespace Ozon.ReportProvider.Bll.Services;

public class ApiReportServiceFake(
    ILogger<ApiReportServiceFake> logger
    ) : IApiReportService
{
    private const int FakeDelay = 1500;
    
    public async Task<Report[]> GetReports(ApiGetReportModel[] models, CancellationToken token)
    {
        logger.LogInformation($"Api was called with batch size:{models.Length}");
        await Task.Delay(FakeDelay, token);
        
        return models.Select(x => new Report
        {
            RequestId = x.RequestId,
            ConversionRatio = (decimal)Random.Shared.NextDouble(),
            SoldCount = Random.Shared.NextInt64(0, 100_000_000),
        }).ToArray();
    }
}