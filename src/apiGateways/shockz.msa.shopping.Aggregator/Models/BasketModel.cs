namespace shockz.msa.shopping.Aggregator.Models
{
  public class BasketModel
  {
    public string UserName { get; set; }
    public List<BasketItemExtendedModel> Items { get; set; } = new List<BasketItemExtendedModel>();
  }
}
