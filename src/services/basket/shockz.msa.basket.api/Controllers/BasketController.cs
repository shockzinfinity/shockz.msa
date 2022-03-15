using Microsoft.AspNetCore.Mvc;
using shockz.msa.basket.api.Entities;
using shockz.msa.basket.api.GrpcServices;
using shockz.msa.basket.api.Repositories;
using System.Net;

namespace shockz.msa.basket.api.Controllers
{
  [Route("api/v1/[controller]")]
  [ApiController]
  public class BasketController : ControllerBase
  {
    private readonly IBasketRepository _basketRepository;
    private readonly DiscountGrpcService _discountGrpcService;

    public BasketController(IBasketRepository basketRepository, DiscountGrpcService discountGrpcService)
    {
      _basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
      _discountGrpcService = discountGrpcService ?? throw new ArgumentNullException(nameof(discountGrpcService));
    }

    [HttpGet("{userName}", Name = "GetBasket")]
    [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
    {
      var basket = await _basketRepository.GetBasket(userName);

      return Ok(basket ?? new ShoppingCart(userName));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
    {
      // NOTE: 1. Communicate with Discount.Grpc
      // NOTE: 2. Calculate latest prices of product into the shopping cart.

      // consume Discount Grpc
      foreach (var item in basket.Items) {
        var coupon = await _discountGrpcService.GetDiscount(item.ProductName);
        item.Price -= coupon.Amount;
      }

      return Ok(await _basketRepository.UpdateBasket(basket));
    }

    [HttpDelete("{userName}", Name = "DeleteBasket")]
    public async Task<IActionResult> DeleteBasket(string userName)
    {
      await _basketRepository.DeleteBasket(userName);

      return Ok();
    }
  }
}
