using Contracts;
using Entities.Models;
using Moq;
using Shared.RequestFeatures;

namespace Test;

public class CompanyRepositoryTests
{
    [Fact]
    public void GetAllCompaniesAsync_ReturnsListOfCompanies_WithASingleCompany()
    {
        var parameters = new CompanyParameters() { PageNumber = 1, PageSize = 1 };
        // Arrange
        var mockRepo = new Mock<ICompanyRepository>();
        mockRepo.Setup(repo => (repo.GetAllCompaniesAsync(parameters, false)))
        .Returns(Task.FromResult(GetCompanies()));
        // Act
        var result = mockRepo.Object.GetAllCompaniesAsync(parameters, false)
        .GetAwaiter()
        .GetResult()
        .ToList();
        // Assert
        Assert.IsType<List<Company>>(result);
        Assert.Single(result);
    }

    public PagedList<Company> GetCompanies()
    {
        return new PagedList<Company>
        (
            new List<Company>()
            {
                new Company
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Company",
                    Country = "United States",
                    Address = "908 Woodrow Way"
                }
            },
            count: 1,
            pageNumber: 1,
            pageSize: 1
            
        );
    }
}