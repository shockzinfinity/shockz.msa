using Npgsql;
using Polly;

namespace shockz.msa.discount.grpc.Extensions;

public static class HostExtensions
{
  public static IHost MigrateDatabase<TContext>(this IHost host)
  {
    using var scope = host.Services.CreateScope();
    var services = scope.ServiceProvider;
    var configuration = services.GetRequiredService<IConfiguration>();
    var logger = services.GetRequiredService<ILogger<TContext>>();

    try {
      logger.LogInformation("Migrating postgresql database.");

      var retry = Policy.Handle<NpgsqlException>()
        .WaitAndRetry(
          retryCount: 5,
          sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
          onRetry: (exception, retryCount, context) =>
          {
            logger.LogError($"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to: {exception}");
          });

      retry.Execute(() => ExecuteMigrations(configuration));

      logger.LogInformation("Migrated postgresql database.");
    } catch (NpgsqlException ex) {
      logger.LogError(ex, "An error occurred while migrating the postgresql database");
    }

    return host;
  }

  private static void ExecuteMigrations(IConfiguration configuration)
  {
    using var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
    connection.Open();

    using var command = connection.CreateCommand();
    command.CommandText = "DROP TABLE IF EXISTS Coupon";
    command.ExecuteNonQuery();

    command.CommandText = @"CREATE TABLE Coupon (Id SERIAL PRIMARY KEY NOT NULL,
                                                    ProductName VARCHAR(24) NOT NULL,
                                                    Description TEXT,
                                                    Amount INT);";
    command.ExecuteNonQuery();

    command.CommandText = "INSERT INTO Coupon (ProductName, Description, Amount) VALUES ('IPhone X', 'IPhone Discount', 150);";
    command.ExecuteNonQuery();

    command.CommandText = "INSERT INTO Coupon (ProductName, Description, Amount) VALUES ('Samsung 10', 'Samsung Discount', 100);";
    command.ExecuteNonQuery();
  }
}
