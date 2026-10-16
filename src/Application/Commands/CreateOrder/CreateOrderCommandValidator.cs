using FluentValidation;
using Application.Commands.CreateOrder;

namespace Application.Commands.CreateOrder
{
    public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(c => c.ClienteId)
                .NotEmpty();

            RuleFor(c => c.Itens)
                .NotEmpty()
                .Must(i => i != null && i.Any())
                .WithMessage("O pedido deve conter ao menos um item.");

            RuleForEach(c => c.Itens).ChildRules(item =>
            {
                item.RuleFor(i => i.ProdutoId).NotEmpty();
                item.RuleFor(i => i.Quantidade).GreaterThan(0);
                item.RuleFor(i => i.PrecoUnitario).GreaterThan(0);
            });
        }
    }
}
