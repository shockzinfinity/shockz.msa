using Microsoft.AspNetCore.Mvc;
using shockz.msa.shopping.Aggregator.Models;
using shockz.msa.shopping.Aggregator.Services;
using System.Net;

namespace shockz.msa.shopping.Aggregator.Controllers
{
  [Route("api/v1/[controller]")]
  [ApiController]
  public class ShoppingController : ControllerBase
  {
    private readonly ICatalogService _catalogService;
    private readonly IBasketService _basketService;
    private readonly IOrderingService _orderingService;
    private readonly ILogger<ShoppingController> _logger;

    public ShoppingController(ICatalogService catalogService, IBasketService basketService, IOrderingService orderingService, ILogger<ShoppingController> logger)
    {
      _catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
      _basketService = basketService ?? throw new ArgumentNullException(nameof(basketService));
      _orderingService = orderingService ?? throw new ArgumentNullException(nameof(orderingService));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("{userName}", Name = "GetShopping")]
    [ProducesResponseType(typeof(ShoppingModel), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ShoppingModel>> GetShopping(string userName)
    {
      // get basket with username
      // iterate basket items and consume products with basket item productId member
      // map product related members into basketitem dto with extended columns
      // consume ordering microservices in order to retrieve order list
      // return root ShoppingModel dto class with including all response

      var basket = await _basketService.GetBasket(userName);

      foreach (var item in basket.Items) {
        var product = await _catalogService.GetCatalogById(item.ProductId);

        // set additional product field onto basket item
        item.ProductName = product.Name;
        item.Category = product.Category;
        item.Summary = product.Summary;
        item.Description = product.Description;
        item.ImageFile = product.ImageFile;
      }

      var orders = await _orderingService.GetOrdersByUserName(userName);

      var shoppingModel = new ShoppingModel
      {
        UserName = userName,
        BasketWithProducts = basket,
        Orders = orders
      };

      return Ok(shoppingModel);
    }
  }
}
