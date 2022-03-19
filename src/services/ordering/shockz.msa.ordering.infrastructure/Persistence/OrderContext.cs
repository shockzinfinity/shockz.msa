using Microsoft.EntityFrameworkCore;
using shockz.msa.ordering.domain.Common;
using shockz.msa.ordering.domain.Entities;

namespace shockz.msa.ordering.infrastructure.Persistence
{
  public class OrderContext : DbContext
  {
    public OrderContext(DbContextOptions<OrderContext> options) : base(options) { }

    public DbSet<Order> Orders { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
      foreach (var entry in ChangeTracker.Entries<EntityBase>()) {
        switch (entry.State) {
          case EntityState.Modified:
            entry.Entity.LastModifiedDate = DateTime.Now;
            entry.Entity.LastModifiedBy = "shockz";
            break;
          case EntityState.Added:
            entry.Entity.CreatedDate = DateTime.Now;
            entry.Entity.CreatedBy = "shockz"; // TODO: forwarding userId
            break;
        }
      }

      return base.SaveChangesAsync(cancellationToken);
    }
  }
}
