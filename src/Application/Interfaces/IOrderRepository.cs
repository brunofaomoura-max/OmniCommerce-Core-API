using Application.Interfaces;
using Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IOrderRepository
    {
        Task AddAsync(Pedido pedido);
        Task<Pedido?> GetByIdAsync(Guid id);
    }
}
