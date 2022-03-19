using AutoMapper;
using MediatR;
using shockz.msa.ordering.application.Contracts.Persistence;

namespace shockz.msa.ordering.application.Features.Orders.Queries.GetOrdersList
{
  public class GetOrdersListQueryHandler : IRequestHandler<GetOrdersListQuery, List<OrdersViewModel>>
  {
    private readonly IOrderingRepository _repository;
    private readonly IMapper _mapper;

    public GetOrdersListQueryHandler(IOrderingRepository repository, IMapper mapper)
    {
      _repository = repository ?? throw new ArgumentNullException(nameof(repository));
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<List<OrdersViewModel>> Handle(GetOrdersListQuery request, CancellationToken cancellationToken)
    {
      var orderList = await _repository.GetOrderByUserName(request.UserName);

      return _mapper.Map<List<OrdersViewModel>>(orderList);
    }
  }
}
