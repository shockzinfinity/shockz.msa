using Microsoft.Extensions.Logging;
using shockz.msa.ordering.domain.Entities;

namespace shockz.msa.ordering.infrastructure.Persistence
{
  public class OrderContextSeed
  {
    public static async Task SeedAsync(OrderContext context, ILogger<OrderContextSeed> logger)
    {
      if (!context.Orders.Any()) {
        context.Orders.AddRange(GetPreconfiguredOrder());
        await context.SaveChangesAsync();
        logger.LogInformation("Seed database associated with context {DbContextName}", typeof(OrderContext).Name);
      }
    }

    private static IEnumerable<Order> GetPreconfiguredOrder()
    {
      return new List<Order>
      {
        new Order { UserName = "shockz", FirstName = "Jun", LastName = "Yu", EmailAddress = "temp@temp.com", AddressLine = "Mapo-gu", Country = "Korea", State = "Seoul", ZipCode = "03936", TotalPrice = 350000 }
      };
    }
  }
}
