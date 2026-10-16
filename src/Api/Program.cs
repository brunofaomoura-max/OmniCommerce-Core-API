using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using MediatR;
using Application.Pipeline;
using FluentValidation;
using Domain.Events;

var builder = WebApplication.CreateBuilder(args);

// ---------- Logging (Serilog) ----------
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// ---------- Services ----------
builder.Services.AddControllers();

// MediatR & pipeline validation
builder.Services.AddMediatR(typeof(Application.Commands.CreateOrder.CreateOrderCommand));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddValidatorsFromAssemblyContaining<Application.Commands.CreateOrder.CreateOrderCommandValidator>();

// EF Core (SQL Server example)
builder.Services.AddDbContext<ApplicationDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dapper context
builder.Services.AddSingleton<DapperContext>();

// Repositories & Unit of Work
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<OrderQueryRepository>();

// JWT authentication (placeholder values – configure in appsettings.json)
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(opts =>
    {
        opts.Authority = builder.Configuration["Jwt:Authority"];
        opts.Audience = builder.Configuration["Jwt:Audience"];
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure global exception middleware
app.UseMiddleware<Api.Middleware.GlobalExceptionHandlerMiddleware>();

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Wire MediatR dispatcher for domain events
DomainEventDispatcher.Mediator = app.Services.GetRequiredService<IMediator>();

app.Run();
