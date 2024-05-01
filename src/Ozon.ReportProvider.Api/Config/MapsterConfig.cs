using Mapster;
using Ozon.ReportProvider.Domain.ValueTypes;
using Decimal = Google.Type.Decimal;

namespace Ozon.ReportProvider.Api.Config;

public static class MapsterConfig
{
    public static void ConfigureApiMapping()
    {
        TypeAdapterConfig<decimal, Decimal>.NewConfig()
            .MapWith(src => Decimal.FromClrDecimal(src));
        TypeAdapterConfig<Decimal, decimal>.NewConfig()
            .MapWith(src => src.ToClrDecimal());
    }
}