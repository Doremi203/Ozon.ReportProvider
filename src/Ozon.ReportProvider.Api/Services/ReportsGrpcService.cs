using Grpc.Core;
using Mapster;
using Ozon.ReportProvider.Domain.Interfaces.Services;
using Ozon.ReportProvider.Domain.ValueTypes;
using Ozon.ReportProvider.Proto;

namespace Ozon.ReportProvider.Api.Services;

public class ReportsGrpcService(
    IReportService reportService
) : ReportsService.ReportsServiceBase
{
    public override async Task<GetReportsResponseV1> GetReportsV1(
        GetReportsRequestV1 request,
        ServerCallContext context
    )
    {
        var reports = await reportService.GetReports(request.RequestIds.Adapt<RequestId[]>(), default);

        return new GetReportsResponseV1
        {
            Reports = { reports.Adapt<ReportV1[]>() }
        };
    }
}