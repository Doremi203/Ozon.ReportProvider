using Mapster;
using Ozon.ReportProvider.Api.Contracts;
using Ozon.ReportProvider.Domain.Entities;
using Ozon.ReportProvider.Domain.Events;
using Ozon.ReportProvider.Domain.Models;
using Ozon.ReportProvider.Domain.ValueTypes;

namespace Ozon.ReportProvider.Api.Config;

public static class MapsterConfig
{
    public static void Configure()
    {
        /*TypeAdapterConfig<ReportRequestEventContract, ReportRequestEvent>.NewConfig()
            .Map(dest => dest.RequestId.Value,
                src => src.RequestId)
            .Map(dest => dest.GoodId.Value,
                src => src.GoodId);

        TypeAdapterConfig<Report, ReportEntityV1>.NewConfig()
            .Map(dest => dest.RequestId,
                src => src.RequestId.Value);*/
        
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