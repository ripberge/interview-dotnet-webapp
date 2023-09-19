using TixTrack.WebApiInterview.Entities;
using TixTrack.WebApiInterview.Repositories;

namespace TixTrack.WebApiInterview.Services;

public interface IProductService
{
    Task<string> Create(Product product);
}

public class ProductServiceImpl : IProductService
{
    private ILogger<ProductServiceImpl> _logger { get; set; }
    private IProductRepository _productRepository { get; set; }

    public ProductServiceImpl(
        ILogger<ProductServiceImpl> logger, IProductRepository productRepository) =>
        (_logger, _productRepository) = (logger, productRepository);
    
    public async Task<string> Create(Product product)
    {
        var productId = await _productRepository.Insert(product);
        _logger.LogInformation("Created product with ID {Id}.", productId);
        return productId;
    }
}