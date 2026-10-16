using System;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;
using Dapper;
using Infrastructure.Services;
using System.Collections.Generic;
using System.Data;

namespace Infrastructure.Repositories
{
    // Consulta de leitura usando Dapper – alta performance
    public sealed class OrderQueryRepository
    {
        private readonly DapperContext _dapperContext;

        public OrderQueryRepository(DapperContext dapperContext) => _dapperContext = dapperContext;

        public async Task<Pedido?> GetOrderWithItensAsync(Guid pedidoId)
        {
            const string sqlPedido = @"SELECT Id, ClienteId, DataCriacao, Status, ValorTotal FROM Pedidos WHERE Id = @Id";
            const string sqlItens = @"SELECT Id, ProdutoId, Quantidade, PrecoUnitario FROM ItensPedido WHERE PedidoId = @Id";

            using IDbConnection conn = _dapperContext.CreateConnection();
            var pedido = await conn.QuerySingleOrDefaultAsync<Pedido>(sqlPedido, new { Id = pedidoId });
            if (pedido == null) return null;

            var itens = await conn.QueryAsync<ItemPedido>(sqlItens, new { Id = pedidoId });
            // Injeta itens privados via reflexão (evita expor setter público)
            var field = typeof(Pedido).GetField("_itens", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(pedido, new List<ItemPedido>(itens));

            return pedido;
        }

        // Exemplo de relatório simples
        public async Task<IEnumerable<(string ClienteId, decimal Total)>> GetTotalPorClienteAsync()
        {
            const string sql = @"SELECT ClienteId, SUM(ValorTotal) AS Total FROM Pedidos GROUP BY ClienteId";
            using IDbConnection conn = _dapperContext.CreateConnection();
            return await conn.QueryAsync<(string ClienteId, decimal Total)>(sql);
        }
    }
}
