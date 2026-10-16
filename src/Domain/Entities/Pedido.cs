using System;
using System.Collections.Generic;
using Domain.Events;

namespace Domain.Entities
{
    public class Pedido
    {
        public Guid Id { get; private set; }
        public string ClienteId { get; private set; }
        public DateTime DataCriacao { get; private set; }
        public string Status { get; private set; }
        public decimal ValorTotal { get; private set; }

        private readonly List<ItemPedido> _itens = new();
        public IReadOnlyCollection<ItemPedido> Itens => _itens.AsReadOnly();

        private Pedido() { }

        public static Pedido Criar(string clienteId, IEnumerable<ItemPedido> itens)
        {
            var pedido = new Pedido
            {
                Id = Guid.NewGuid(),
                ClienteId = clienteId,
                DataCriacao = DateTime.UtcNow,
                Status = "Criado"
            };

            foreach (var item in itens)
                pedido.AdicionarItem(item);

            DomainEventDispatcher.Raise(new PedidoCriadoEvent(pedido.Id, pedido.ClienteId));

            return pedido;
        }

        public void AdicionarItem(ItemPedido item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            _itens.Add(item);
            RecalcularValorTotal();
        }

        public void AlterarStatus(string novoStatus)
        {
            if (string.IsNullOrWhiteSpace(novoStatus))
                throw new ArgumentException("Status inválido.", nameof(novoStatus));

            var statusPossiveis = new[] { "Criado", "Pago", "Enviado", "Cancelado" };
            if (!Array.Exists(statusPossiveis, s => s == novoStatus))
                throw new InvalidOperationException($"Transição para status '{novoStatus}' não permitida.");

            Status = novoStatus;
        }

        private void RecalcularValorTotal()
        {
            ValorTotal = 0m;
            foreach (var i in _itens)
                ValorTotal += i.PrecoUnitario * i.Quantidade;
        }
    }
}
