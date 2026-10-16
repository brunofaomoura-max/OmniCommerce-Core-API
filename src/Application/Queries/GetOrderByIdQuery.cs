using MediatR;
using System;

namespace Application.Queries
{
    // Query to retrieve an order by its Id.
    public sealed record GetOrderByIdQuery(Guid Id) : IRequest<PedidoDto>;
}
