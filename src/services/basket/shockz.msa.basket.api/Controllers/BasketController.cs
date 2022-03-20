using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using shockz.msa.basket.api.Entities;
using shockz.msa.basket.api.GrpcServices;
using shockz.msa.basket.api.Repositories;
using shockz.msa.eventBus.messages.Events;
using System.Net;

namespace shockz.msa.basket.api.Controllers
{
  [Route("api/v1/[controller]")]
  [ApiController]
  public class BasketController : ControllerBase
  {
    private readonly IBasketRepository _basketRepository;
    private readonly DiscountGrpcService _discountGrpcService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IMapper _mapper;

    public BasketController(IBasketRepository basketRepository, DiscountGrpcService discountGrpcService, IPublishEndpoint publishEndpoint, IMapper mapper)
    {
      _basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
      _discountGrpcService = discountGrpcService ?? throw new ArgumentNullException(nameof(discountGrpcService));
      _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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

    [Route("[action]")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
    {
      // 1. get existing basket with total price
      // 2. create basketcheckoutevent -- set total price on basketcheckout event message
      // 3. send checkout event to rabbitmq
      // 4. remove the basket

      // 1.
      var basket = await _basketRepository.GetBasket(basketCheckout.UserName);
      if (basket == null)
        return BadRequest();

      // 2, 3.
      var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
      eventMessage.TotalPrice = basket.TotalPrice;
      await _publishEndpoint.Publish(eventMessage);

      // 4.
      await _basketRepository.DeleteBasket(basket.UserName);

      return Accepted();
    }
  }
}
