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
    public async Task GetReport_ReportExists_ReturnsReport()
    {
        // Arrange
        var reports = ReportEntityV1Faker.Generate();
        var ids = reports.Select(x => new RequestId(x.RequestId)).ToArray();
        await _reportRepository.Add(reports, CancellationToken.None);

        // Act
        var actual = await _reportRepository.GetReport(ids[0], CancellationToken.None);

        // Assert
        actual.Should().BeEquivalentTo(reports[0]);
    }

    [Fact]
    public async Task GetReport_NoReport_ReturnsNull()
    {
        // Arrange
        var id = new RequestId(-1);
        
        // Act
        var actual = await _reportRepository.GetReport(id, CancellationToken.None);
        
        // Assert
        actual.Should().BeNull();
    }

    [Fact]
    public async Task GetCompletedRequestIds_Success()
    {
        // Arrange
        var reports = ReportEntityV1Faker.Generate(5);
        await _reportRepository.Add(reports, CancellationToken.None);
        var existingIds = reports.Select(x => new RequestId(x.RequestId)).ToArray();
        var expectedIds = existingIds.Take(2).ToArray();
        var ids = new List<RequestId> { new(-1), new(-2), new(-3) };
        ids.AddRange(expectedIds);
        
        // Act
        var actual = await _reportRepository.GetCompletedRequestIds(ids.ToArray(), CancellationToken.None);
        
        // Assert
        actual.Should().BeEquivalentTo(expectedIds);
    }
}