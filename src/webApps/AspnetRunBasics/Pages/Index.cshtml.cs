﻿using AspnetRunBasics.Models;
using AspnetRunBasics.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspnetRunBasics.Pages
{
  public class IndexModel : PageModel
  {
    private readonly ICatalogService _catalogService;
    private readonly IBasketService _basketService;

    public IndexModel(ICatalogService catalogService, IBasketService basketService)
    {
      _catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
      _basketService = basketService ?? throw new ArgumentNullException(nameof(basketService));
    }

    public IEnumerable<CatalogModel> ProductList { get; set; } = new List<CatalogModel>();

    public async Task<IActionResult> OnGetAsync()
    {
      ProductList = await _catalogService.GetCatalog();
      return Page();
    }

    public async Task<IActionResult> OnPostAddToCartAsync(string productId)
    {
      var product = await _catalogService.GetCatalogById(productId);
      var userName = "shockz";
      var basket = await _basketService.GetBasket(userName);

      basket.Items.Add(new BasketItemModel
      {
        ProductId = productId,
        ProductName = product.Name,
        Price = product.Price,
        Quantity = 1,
        Color = "Black"
      });

      return RedirectToPage("Cart");
    }
  }
}
