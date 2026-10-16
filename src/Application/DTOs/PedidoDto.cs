using System;
using System.Collections.Generic;

namespace Application.DTOs
{
    // DTO usado para retornar os dados de um pedido via API.
    public sealed class PedidoDto
    {
        public Guid Id { get; init; }
        public string ClienteId { get; init; } = default!;
        public DateTime DataCriacao { get; init; }
        public string Status { get; init; } = default!;
        public decimal ValorTotal { get; init; }
        public IReadOnlyCollection<ItemPedidoDto> Itens { get; init; } = Array.Empty<ItemPedidoDto>();
    }

    public sealed class ItemPedidoDto
    {
        public Guid Id { get; init; }
        public Guid ProdutoId { get; init; }
        public int Quantidade { get; init; }
        public decimal PrecoUnitario { get; init; }
    }
}
