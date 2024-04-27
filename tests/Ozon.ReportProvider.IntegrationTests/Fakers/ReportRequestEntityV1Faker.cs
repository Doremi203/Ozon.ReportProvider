using AutoBogus;
using Bogus;
using Ozon.ReportProvider.Domain.Entities;
using Ozon.ReportProvider.IntegrationTests.Creators;

namespace Ozon.ReportProvider.IntegrationTests.Fakers;

public static class ReportRequestEntityV1Faker
{
    private static readonly object Lock = new();

    private static readonly Faker<ReportRequestEntityV1> Faker = new AutoFaker<ReportRequestEntityV1>()
        .RuleFor(x => x.RequestId, f => f.Random.Guid())
        .RuleFor(x => x.GoodId, f => f.Random.Guid())
        .RuleFor(x => x.StartOfPeriod, f => f.Date.RecentOffset().UtcDateTime)
        .RuleFor(x => x.EndOfPeriod, f => f.Date.RecentOffset().UtcDateTime)
        .RuleFor(x => x.CreatedAt, f => f.Date.RecentOffset().UtcDateTime);

    public static ReportRequestEntityV1[] Generate(int count = 1)
    {
        lock (Lock)
        {
            var res = Faker.Generate(count).ToArray();
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = res[i].WithPeriod(Create.RandomPeriod());
            }
            
            return res;
        }
    }
    
    public static ReportRequestEntityV1 WithRequestId(
        this ReportRequestEntityV1 src,
        Guid requestId)
        => src with { RequestId = requestId };
    
    public static ReportRequestEntityV1 WithGoodId(
        this ReportRequestEntityV1 src,
        Guid goodId)
        => src with { GoodId = goodId };
    
    public static ReportRequestEntityV1 WithPeriod(
        this ReportRequestEntityV1 src,
        (DateTimeOffset start, DateTimeOffset end) period)
        => src with { StartOfPeriod = period.start, EndOfPeriod = period.end };
}