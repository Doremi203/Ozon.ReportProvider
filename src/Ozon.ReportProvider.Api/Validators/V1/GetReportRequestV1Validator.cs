using FluentValidation;
using Ozon.ReportProvider.Proto;

namespace Ozon.ReportProvider.Api.Validators.V1;

public class GetReportRequestV1Validator : AbstractValidator<GetReportRequestV1>
{
    public GetReportRequestV1Validator()
    {
        RuleFor(x => x.RequestId).NotEmpty();
    }
}