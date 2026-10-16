using System;
using System.Collections.Generic;
using MediatR;

namespace Application.Commands.CreateOrder
{
    public sealed record CreateOrderCommand(
        string ClienteId,
        IEnumerable<CreateOrderItemDto> Itens) : IRequest<Guid>;

    public sealed record CreateOrderItemDto(
        Guid ProdutoId,
        int Quantidade,
        decimal PrecoUnitario);
}
