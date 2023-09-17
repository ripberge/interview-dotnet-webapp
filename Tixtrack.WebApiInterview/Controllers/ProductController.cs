﻿using Microsoft.AspNetCore.Mvc;
using TixTrack.WebApiInterview.Entities;
using TixTrack.WebApiInterview.Repositories;

namespace TixTrack.WebApiInterview.Controllers;

[ApiController]
public class ProductController : ControllerBase
{
    private IProductRepository _productRepository { get; set; }

    public ProductController(IProductRepository productRepository) =>
        _productRepository = productRepository;

    [HttpGet]
    [Route("product/{productId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Product>> GetById(int productId)
    {
        var product = await _productRepository.FindById(productId);
        return product == null ? NotFound() : Ok(product);
    }
}