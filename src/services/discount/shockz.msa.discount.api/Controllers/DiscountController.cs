using Microsoft.AspNetCore.Mvc;
using shockz.msa.discount.api.Entities;
using shockz.msa.discount.api.Repositories;
using System.Net;

namespace shockz.msa.discount.api.Controllers
{
  [Route("api/v1/[controller]")]
  [ApiController]
  public class DiscountController : ControllerBase
  {
    private readonly IDiscountRepository _repository;

    public DiscountController(IDiscountRepository repository)
    {
      _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    [HttpGet("{productName}", Name = "GetDiscount")]
    [ProducesResponseType(typeof(Coupon), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Coupon>> GetDiscount(string productName)
    {
      var discount = await _repository.GetDiscount(productName);

      return Ok(discount);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Coupon), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Coupon>> CreateDiscount([FromBody] Coupon coupon)
    {
      await _repository.CreateDiscount(coupon);

      return CreatedAtRoute("GetDiscount", new { productName = coupon.ProductName }, coupon);
    }

    [HttpPut]
    [ProducesResponseType(typeof(Coupon), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Coupon>> UpdateDiscount([FromBody] Coupon coupon)
    {
      return Ok(await _repository.UpdateDiscount(coupon));
    }

    [HttpDelete("{productName}", Name = "DeleteDiscount")]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> DeleteDiscount(string productName)
    {
      return Ok(await _repository.DeleteDiscount(productName));
    }
  }
}
