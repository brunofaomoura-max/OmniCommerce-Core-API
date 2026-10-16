using MediatR;
using System;

namespace Domain.Events
{
    // Evento de domínio publicado quando um pedido é criado.
    public sealed class PedidoCriadoEvent : INotification
    {
        public Guid PedidoId { get; }
        public string ClienteId { get; }

        public PedidoCriadoEvent(Guid pedidoId, string clienteId)
        {
            PedidoId = pedidoId;
            ClienteId = clienteId;
        }
    }

    // Dispatcher simples que será injetado em Program.cs.
    internal static class DomainEventDispatcher
    {
        public static IMediator? Mediator { get; set; }
        public static void Raise<TEvent>(TEvent @event) where TEvent : INotification
        {
            Mediator?.Publish(@event);
        }
    }
}
