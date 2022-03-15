using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace shockz.msa.catalog.api.Entities
{
  public class Price
  {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    [BsonRepresentation(BsonType.ObjectId)]
    public string ProductId { get; set; }
    public decimal Value { get; set; }
    public string Supplier { get; set; }
  }
}
