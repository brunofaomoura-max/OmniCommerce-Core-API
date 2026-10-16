using System;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs;
using MediatR;
using Infrastructure.Repositories;

namespace Application.Queries
{
    // Handler for GetOrderByIdQuery – returns a DTO for the API layer.
    public sealed class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, PedidoDto?>
    {
        private readonly OrderQueryRepository _queryRepository;

        public GetOrderByIdHandler(OrderQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }

        public async Task<PedidoDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var pedido = await _queryRepository.GetOrderWithItensAsync(request.Id);
            if (pedido == null) return null;

            // Map domain entity to DTO – manual mapping to keep the example lightweight.
            var dto = new PedidoDto
            {
                Id = pedido.Id,
                ClienteId = pedido.ClienteId,
                DataCriacao = pedido.DataCriacao,
                Status = pedido.Status,
                ValorTotal = pedido.ValorTotal,
                Itens = System.Linq.Enumerable.Select(pedido.Itens, i => new ItemPedidoDto
                {
                    Id = i.Id,
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario
                }).ToArray()
            };

            return dto;
        }
    }
}
