using Mapster;
using Ozon.ReportProvider.Domain.ValueTypes;
using Decimal = Google.Type.Decimal;

namespace Ozon.ReportProvider.Api.Config;

public static class MapsterConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<decimal, Decimal>.NewConfig()
            .MapWith(src => Decimal.FromClrDecimal(src));
        TypeAdapterConfig<Decimal, decimal>.NewConfig()
            .MapWith(src => src.ToClrDecimal());
        
        TypeAdapterConfig<long, RequestId>.NewConfig()
            .MapWith(src => new RequestId(src));
        TypeAdapterConfig<RequestId, long>.NewConfig()
            .Map(dest => dest,
                src => src.Value);
        TypeAdapterConfig<long, GoodId>.NewConfig()
            .Map(dest => dest.Value,
                src => src);
        TypeAdapterConfig<GoodId, long>.NewConfig()
            .Map(dest => dest,
                src => src.Value);
    }
}