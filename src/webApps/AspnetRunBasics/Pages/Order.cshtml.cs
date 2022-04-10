using AspnetRunBasics.Models;
using AspnetRunBasics.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspnetRunBasics
{
  public class OrderModel : PageModel
  {
    private readonly IOrderService _orderService;

    public OrderModel(IOrderService orderService)
    {
      _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
    }

    public IEnumerable<OrderResponseModel> Orders { get; set; } = new List<OrderResponseModel>();

    public async Task<IActionResult> OnGetAsync()
    {
      var userName = "shockz";
      Orders = await _orderService.GetOrderByUserName(userName);

      return Page();
    }
  }
}
