using Grpc.Core;
using Mapster;
using Ozon.ReportProvider.Domain.Exceptions;
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
        try
        {
            var report = await reportService.GetReport(request.RequestId.Adapt<RequestId>(), context.CancellationToken);
            return new GetReportResponseV1
            {
                Report = report.Adapt<ReportV1>()
            };
        }
        catch (ReportNotReadyException e)
        {
            throw new RpcException(new Status(StatusCode.Unavailable, e.Message));
        }
    }
}