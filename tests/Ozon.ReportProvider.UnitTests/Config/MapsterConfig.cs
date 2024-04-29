using Mapster;
using Ozon.ReportProvider.Domain.ValueTypes;

namespace Ozon.ReportProvider.UnitTests.Config;

public static class MapsterConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<long, RequestId>.NewConfig()
            .Map(dest => dest.Value,
                src => src);
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