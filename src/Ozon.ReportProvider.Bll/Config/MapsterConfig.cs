using Mapster;
using Ozon.ReportProvider.Domain.ValueTypes;

namespace Ozon.ReportProvider.Bll.Config;

public static class MapsterConfig
{
    public static void ConfigureDomainMapping()
    {
        TypeAdapterConfig<long, RequestId>.NewConfig()
            .MapWith(src => new RequestId(src));
        TypeAdapterConfig<RequestId, long>.NewConfig()
            .MapWith(src => src.Value);

        TypeAdapterConfig<long, GoodId>.NewConfig()
            .MapWith(src => new GoodId(src));
        TypeAdapterConfig<GoodId, long>.NewConfig()
            .MapWith(src => src.Value);
    }
}