using FluentAssertions;
using Ozon.ReportProvider.Domain.Interfaces.Repositories;
using Ozon.ReportProvider.IntegrationTests.Fakers;
using Ozon.ReportProvider.IntegrationTests.Fixtures;

namespace Ozon.ReportProvider.IntegrationTests.RepositoriesTests;

[Collection(nameof(DalTestFixture))]
public class ReportRequestRepositoryTests(DalTestFixture fixture)
{
    private IReportRequestRepository _repository = fixture.ReportRequestRepository;
    
    [Fact]
    public async Task Add_ReportRequest_Success()
    {
        // Arrange
        var reportRequest = ReportRequestEntityV1Faker.Generate();
        
        // Act
        var ids = await _repository.Add(reportRequest, default);
        var result = await _repository.GetById(ids[0], default);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(reportRequest[0].WithId(ids[0]));
    }
}