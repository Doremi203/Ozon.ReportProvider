using AutoBogus;
using Bogus;
using Ozon.ReportProvider.Domain.Entities;
using Ozon.ReportProvider.IntegrationTests.Creators;

namespace Ozon.ReportProvider.IntegrationTests.Fakers;

public static class ReportEntityV1Faker
{
    private static readonly object Lock = new();

    private static readonly Faker<ReportEntityV1> Faker = new AutoFaker<ReportEntityV1>()
        .RuleFor(x => x.RequestId, _ => Create.RandomId())
        .RuleFor(x => x.ConversionRatio, f => f.Random.Decimal())
        .RuleFor(x => x.SoldCount, f => f.Random.Int(0, 1000));

    public static ReportEntityV1[] Generate(int count = 1)
    {
        lock (Lock)
        {
            return Faker.Generate(count).ToArray();
        }
    }
    
    public static ReportEntityV1 WithId(
        this ReportEntityV1 src, 
        long id)
        => src with { RequestId = id };
    
    public static ReportEntityV1 WithConversionRatio(
        this ReportEntityV1 src, 
        decimal conversionRatio)
        => src with { ConversionRatio = conversionRatio };
    
    public static ReportEntityV1 WithSoldCount(
        this ReportEntityV1 src, 
        int soldCount)
        => src with { SoldCount = soldCount };
}