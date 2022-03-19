using MediatR;

namespace shockz.msa.ordering.application.Features.Orders.Commands.UpdateOrder
{
  public class UpdateOrderCommand : IRequest // because of returning nothing
  {
    public int Id { get; set; }
    public string UserName { get; set; }
    public decimal TotalPrice { get; set; }

    // billing address
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string AddressLine { get; set; }
    public string Country { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }

    // payment
    public string CardName { get; set; }
    public string CardNumber { get; set; }
    public string Expiration { get; set; }
    public string CVV { get; set; }
    public int PaymentMethod { get; set; }
  }
}
