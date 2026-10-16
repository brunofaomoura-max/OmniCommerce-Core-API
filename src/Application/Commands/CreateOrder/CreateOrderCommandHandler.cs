using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Commands.CreateOrder
{
    public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrderCommandHandler(IOrderRepository orderRepository,
                                         IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateOrderCommand request,
                                       CancellationToken cancellationToken)
        {
            var itens = request.Itens.Select(i =>
                new ItemPedido(i.ProdutoId, i.Quantidade, i.PrecoUnitario));

            var pedido = Pedido.Criar(request.ClienteId, itens);

            await _orderRepository.AddAsync(pedido);
            await _unitOfWork.CommitAsync();

            return pedido.Id;
        }
    }
}
