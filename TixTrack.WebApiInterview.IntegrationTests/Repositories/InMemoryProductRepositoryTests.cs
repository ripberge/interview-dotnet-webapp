using TixTrack.WebApiInterview.Entities;
using TixTrack.WebApiInterview.Repositories;
using Xunit;

namespace TixTrack.WebApiInterview.IntegrationTests.Repositories;

public partial class InMemoryProductRepositoryTests
{
    [Fact]
    public async Task ProductIsNotAbsentForExistingId()
    {
        var expectedProduct = _validProduct with { Name = Guid.NewGuid().ToString() };
        var expectedId = await _productRepository.Insert(expectedProduct);

        var actualProduct = await _productRepository.FindById(expectedId);
        
        Assert.Equal(expectedProduct.Name, actualProduct?.Name);
    }

    [Fact]
    public async Task StoredAndRetrievedDataAreNotInconsistent()
    {
        var expectedProduct = _validProduct;
        var expectedId = await _productRepository.Insert(expectedProduct);
        
        var actualProduct = await _productRepository.FindById(expectedId);
        
        Assert.Equal(expectedProduct, actualProduct);
    }
}

public partial class InMemoryProductRepositoryTests
{
    private Product _validProduct => new()
    {
        Id = 10,
        Name = "T-shirt",
        AvailableQuantity = 100,
        Price = 10.50,
        Type = "Clothing"
    };
    
    private InMemoryProductRepository _productRepository { get; set; }

    public InMemoryProductRepositoryTests()
    {
        var db = new ApplicationContext();
        db.Database.EnsureDeleted();
        _productRepository = new InMemoryProductRepository(db);
    }
}