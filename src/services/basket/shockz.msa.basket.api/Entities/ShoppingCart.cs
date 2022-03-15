namespace shockz.msa.basket.api.Entities
{
  public class ShoppingCart
  {
    public string UserName { get; set; }
    public List<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();

    public ShoppingCart() { }

    public ShoppingCart(string userName)
    {
      UserName = userName;
    }

    public decimal TotalPrice
    {
      get
      {
        return RecalculatePrices();
      }
    }

    private decimal RecalculatePrices()
    {
      decimal total = 0;
      foreach (var item in Items) {
        total += item.Price * item.Quantity;
      }

      return total;
    }
  }
}
