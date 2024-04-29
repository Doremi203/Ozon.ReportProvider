using Ozon.ReportProvider.Domain.Interfaces.Services;
using Ozon.ReportProvider.Domain.Models;

namespace Ozon.ReportProvider.Bll.Services;

public class ApiReportServiceFake : IApiReportService
{
    private const int FakeDelay = 1500;
    
    public async Task<Report[]> GetReports(ApiGetReportModel[] models, CancellationToken token)
    {
        await Task.Delay(FakeDelay, token);
        
        return models.Select(x => new Report
        {
            RequestId = x.RequestId,
            ConversionRatio = (decimal)Random.Shared.NextDouble(),
            SoldCount = Random.Shared.NextInt64(0, 100_000_000),
        }).ToArray();
    }
}