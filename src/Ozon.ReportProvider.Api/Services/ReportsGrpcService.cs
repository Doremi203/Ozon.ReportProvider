using Grpc.Core;
using Mapster;
using Ozon.ReportProvider.Domain.Interfaces.Services;
using Ozon.ReportProvider.Domain.ValueTypes;
using Ozon.ReportProvider.Proto;

namespace Ozon.ReportProvider.Api.Services;

public class ReportsGrpcService(
    IReportService reportService
) : ReportService.ReportServiceBase
{
    public override async Task<GetReportResponseV1> GetReportV1(
        GetReportRequestV1 request,
        ServerCallContext context
    )
    {
        var report = await reportService.GetReport(request.RequestId.Adapt<RequestId>(), default);

        return new GetReportResponseV1
        {
            Report = report.Adapt<ReportV1>()
        };
    }
}