using FluentAssertions;
using Ozon.ReportProvider.Domain.Interfaces.Repositories;
using Ozon.ReportProvider.Domain.ValueTypes;
using Ozon.ReportProvider.IntegrationTests.Fakers;
using Ozon.ReportProvider.IntegrationTests.Fixtures;

namespace Ozon.ReportProvider.IntegrationTests.RepositoriesTests;

[Collection(nameof(DalTestFixture))]
public class ReportRepositoryTests(DalTestFixture fixture)
{
    private readonly IReportRepository _reportRepository = fixture.ReportRepository;
    
    [Fact]
    public async Task Add_Success()
    {
        // Arrange
        var reports = ReportEntityV1Faker.Generate(5);
        var ids = reports.Select(x => new RequestId(x.RequestId)).ToArray();

        // Act
        await _reportRepository.Add(reports, CancellationToken.None);

        // Assert
        var actual = await _reportRepository.GetReports(ids, CancellationToken.None);
        actual.Should().BeEquivalentTo(reports);
    }
    
    [Fact]
    public async Task GetReports_Success()
    {
        // Arrange
        var reports = ReportEntityV1Faker.Generate(5);
        var ids = reports.Select(x => new RequestId(x.RequestId)).ToArray();
        await _reportRepository.Add(reports, CancellationToken.None);

        // Act
        var actual = await _reportRepository.GetReports(ids, CancellationToken.None);

        // Assert
        actual.Should().BeEquivalentTo(reports);
    }
}