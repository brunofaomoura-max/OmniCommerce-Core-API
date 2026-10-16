using Application.Commands.CreateOrder;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class PedidosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PedidosController(IMediator mediator) => _mediator = mediator;

        // POST /api/pedidos
        [HttpPost]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Create([FromBody] CreateOrderCommand command)
        {
            var pedidoId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = pedidoId }, new { Id = pedidoId });
        }

        // GET /api/pedidos/{id}
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Cliente,Admin")]
        public async Task<IActionResult> GetById(Guid id)
        {
            // Placeholder – a query handler using OrderQueryRepository would be invoked here.
            return Ok();
        }
    }
}
