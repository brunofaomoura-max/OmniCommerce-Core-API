using System;

namespace Domain.Entities
{
    public class ItemPedido
    {
        public Guid Id { get; private set; }
        public Guid ProdutoId { get; private set; }
        public int Quantidade { get; private set; }
        public decimal PrecoUnitario { get; private set; }

        private ItemPedido() { }

        public ItemPedido(Guid produtoId, int quantidade, decimal precoUnitario)
        {
            if (quantidade <= 0) throw new ArgumentException("Quantidade deve ser > 0.", nameof(quantidade));
            if (precoUnitario <= 0) throw new ArgumentException("Preço unitário deve ser > 0.", nameof(precoUnitario));

            Id = Guid.NewGuid();
            ProdutoId = produtoId;
            Quantidade = quantidade;
            PrecoUnitario = precoUnitario;
        }
    }
}
