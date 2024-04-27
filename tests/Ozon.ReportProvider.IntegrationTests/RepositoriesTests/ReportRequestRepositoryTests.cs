using FluentAssertions;
using Ozon.ReportProvider.Domain.Interfaces.Repositories;
using Ozon.ReportProvider.IntegrationTests.Fakers;
using Ozon.ReportProvider.IntegrationTests.Fixtures;

namespace Ozon.ReportProvider.IntegrationTests.RepositoriesTests;

[Collection(nameof(DalTestFixture))]
public class ReportRequestRepositoryTests(DalTestFixture fixture)
{
    private readonly IReportRequestRepository _repository = fixture.ReportRequestRepository;
    
    [Fact]
    public async Task Add_Success()
    {
        // Arrange
        var reportRequests = ReportRequestEntityV1Faker.Generate(5);
        var ids = reportRequests.Select(x => x.RequestId).ToArray();
        
        // Act
        await _repository.Add(reportRequests, default);
        var result = await _repository.GetByIds(ids, default);
        
        // Assert
        result.Should().BeEquivalentTo(reportRequests);
    }
}