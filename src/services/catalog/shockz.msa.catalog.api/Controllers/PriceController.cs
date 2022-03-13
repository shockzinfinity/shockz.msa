using Microsoft.AspNetCore.Mvc;
using shockz.msa.catalog.api.Entities;
using shockz.msa.catalog.api.Repositories;
using System.Net;

namespace shockz.msa.catalog.api.Controllers
{
  [Route("api/v1/[controller]")]
  [ApiController]
  public class PriceController : ControllerBase
  {
    private readonly IPriceRepository _priceRepository;
    private readonly ILogger<PriceController> _logger;

    public PriceController(IPriceRepository priceRepository, ILogger<PriceController> logger)
    {
      _priceRepository = priceRepository ?? throw new ArgumentNullException(nameof(priceRepository));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Price>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<Price>>> GetPrices(string productId)
    {
      var prices = await _priceRepository.GetPricesByProductId(productId);

      return Ok(prices);
    }
  }
}
