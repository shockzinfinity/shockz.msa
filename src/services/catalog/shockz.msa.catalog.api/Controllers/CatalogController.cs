using Microsoft.AspNetCore.Mvc;
using shockz.msa.catalog.api.Entities;
using shockz.msa.catalog.api.Repositories;
using System.Net;

namespace shockz.msa.catalog.api.Controllers
{
  [Route("api/v1/[controller]")]
  [ApiController]
  public class CatalogController : ControllerBase
  {
    private readonly IProductRepository _productRepository;
    private readonly ILogger<CatalogController> _logger;

    public CatalogController(IProductRepository productRepository, ILogger<CatalogController> logger)
    {
      _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
      var products = await _productRepository.GetProducts();

      return Ok(products);
    }

    [HttpGet("{id:length(24)}", Name = "GetProduct")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Product>> GetProductById(string id)
    {
      var product = await _productRepository.GetProduct(id);
      if (product == null)
      {
        _logger.LogError($"Product with Id:{id}, not found");
        return NotFound();
      }

      return Ok(product);
    }

    [Route("[action]/{category}", Name = "GetProductByCategory")]
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string category)
    {
      var product = await _productRepository.GetProductsByCategory(category);
      return Ok(product);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
    {
      await _productRepository.CreateProduct(product);

      return CreatedAtRoute("GetProduct", new { id = product.Id }, GetProductById);
    }

    [HttpPut]
    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateProduct([FromBody] Product product)
    {
      // use IActionResult because of only return bool

      return Ok(await _productRepository.UpdateProduct(product));
    }

    [HttpDelete("{id:length(24)}", Name = "DeleteProduct")]
    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeleteProduct(string id)
    {
      // use IActionResult because of only return bool
      
      return Ok(await _productRepository.DeleteProduct(id));
    }
  }
}
