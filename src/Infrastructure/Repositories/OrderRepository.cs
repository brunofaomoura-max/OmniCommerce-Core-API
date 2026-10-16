using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public sealed class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context) => _context = context;

        public async Task AddAsync(Pedido pedido)
        {
            await _context.Pedidos.AddAsync(pedido);
        }

        public async Task<Pedido?> GetByIdAsync(Guid id)
        {
            return await _context.Pedidos
                                 .Include(p => p.Itens)
                                 .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
