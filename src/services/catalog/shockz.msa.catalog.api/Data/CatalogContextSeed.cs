using MongoDB.Driver;
using shockz.msa.catalog.api.Entities;

namespace shockz.msa.catalog.api.Data
{
  public class CatalogContextSeed
  {
    public static void SeedData(IMongoCollection<Product> productCollection, IMongoCollection<Price> priceCollection)
    {
      bool existProduct = productCollection.Find(p => true).Any();
      if (!existProduct) {
        productCollection.InsertManyAsync(GetPreconfiguredProducts());
      }

      bool existPrice = priceCollection.Find(p => true).Any();
      if (!existPrice) {
        priceCollection.InsertManyAsync(GetPreconfiguredPrices());
      }
    }

    private static IEnumerable<Price> GetPreconfiguredPrices()
    {
      return new List<Price>()
      {
        new Price()
        {
          Id = "622da15ab43bdef3c4edf5f5",
          ProductId ="602d2149e773f2a3990b47f5",
          Value = 950.00M,
          Supplier = "Apple"
        },
        new Price()
        {
          Id = "622da255761441a1ab05d898",
          ProductId ="602d2149e773f2a3990b47f5",
          Value = 920.00M,
          Supplier = "Apple II"
        },
        new Price()
        {
          Id = "622da1b089844e2ed4107f1c",
          ProductId ="602d2149e773f2a3990b47f6",
          Value = 840.00M,
          Supplier = "Samsung"
        },
        new Price()
        {
          Id = "622da26d07dbd01b7fb198e4",
          ProductId ="602d2149e773f2a3990b47f6",
          Value = 900.00M,
          Supplier = "Samsung II"
        },
        new Price()
        {
          Id = "622da28553d2a2e53361eed5",
          ProductId ="602d2149e773f2a3990b47f6",
          Value = 1040.00M,
          Supplier = "ironPot42.com"
        },
        new Price()
        {
          Id = "622da1df59fa0f393a479470",
          ProductId ="602d2149e773f2a3990b47f7",
          Value = 650.00M,
          Supplier = "Huawei"
        },
        new Price()
        {
          Id = "622da1fee25beaf249c7de96",
          ProductId ="602d2149e773f2a3990b47f8",
          Value = 470.00M,
          Supplier = "Xiaomi"
        },
        new Price()
        {
          Id = "622da2acce1898c5f54cde45",
          ProductId ="602d2149e773f2a3990b47f8",
          Value = 440.00M,
          Supplier = "Xiaomi II"
        },
        new Price()
        {
          Id = "622da21e8008e7ae399a3ce1",
          ProductId ="602d2149e773f2a3990b47f9",
          Value = 380.00M,
          Supplier = "HTC"
        },
        new Price()
        {
          Id = "622da237e2080de35af63477",
          ProductId ="602d2149e773f2a3990b47fa",
          Value = 240.00M,
          Supplier = "LG"
        },
        new Price()
        {
          Id = "622da2d56f8b13035309847d",
          ProductId ="602d2149e773f2a3990b47fa",
          Value = 1240.00M,
          Supplier = "MG"
        }
      };
    }

    private static IEnumerable<Product> GetPreconfiguredProducts()
    {
      return new List<Product>()
      {
        new Product()
        {
          Id = "602d2149e773f2a3990b47f5",
          Name = "IPhone X",
          Summary = "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
          Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus. Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus.",
          ImageFile = "product-1.png",
          Price = 950.00M,
          Category = "Smart Phone"
        },
        new Product()
        {
          Id = "602d2149e773f2a3990b47f6",
          Name = "Samsung 10",
          Summary = "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
          Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus. Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus.",
          ImageFile = "product-2.png",
          Price = 840.00M,
          Category = "Smart Phone"
        },
        new Product()
        {
          Id = "602d2149e773f2a3990b47f7",
          Name = "Huawei Plus",
          Summary = "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
          Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus. Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus.",
          ImageFile = "product-3.png",
          Price = 650.00M,
          Category = "White Appliances"
        },
        new Product()
        {
          Id = "602d2149e773f2a3990b47f8",
          Name = "Xiaomi Mi 9",
          Summary = "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
          Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus. Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus.",
          ImageFile = "product-4.png",
          Price = 470.00M,
          Category = "White Appliances"
        },
        new Product()
        {
          Id = "602d2149e773f2a3990b47f9",
          Name = "HTC U11+ Plus",
          Summary = "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
          Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus. Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus.",
          ImageFile = "product-5.png",
          Price = 380.00M,
          Category = "Smart Phone"
        },
        new Product()
        {
          Id = "602d2149e773f2a3990b47fa",
          Name = "LG G7 ThinQ",
          Summary = "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
          Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus. Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus.",
          ImageFile = "product-6.png",
          Price = 240.00M,
          Category = "Home Kitchen"
        }
      };
    }
  }
}
