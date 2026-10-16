using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation;

namespace Api.Middleware
{
    // Converte exceções em respostas RFC 7807 (ProblemDetails)
    public sealed class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var problem = new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "Um erro inesperado ocorreu.",
                Detail = exception.Message,
                Instance = context.Request.Path
            };

            if (exception is ValidationException validationEx)
            {
                problem.Status = (int)HttpStatusCode.BadRequest;
                problem.Title = "Dados de entrada inválidos.";
                problem.Detail = string.Join("; ", validationEx.Errors);
            }

            var payload = JsonSerializer.Serialize(problem);
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = problem.Status ?? (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync(payload);
        }
    }
}
