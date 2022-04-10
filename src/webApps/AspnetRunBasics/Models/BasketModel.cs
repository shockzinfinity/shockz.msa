using System.Collections.Generic;

namespace AspnetRunBasics.Models;

public class BasketModel
{
  public string UserName { get; set; }
  public List<BasketItemModel> Items { get; set; } = new();
  public decimal TotalPrice { get; set; }
}
