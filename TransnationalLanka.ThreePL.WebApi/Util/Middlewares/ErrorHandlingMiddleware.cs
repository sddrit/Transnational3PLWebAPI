using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TransnationalLanka.ThreePL.Core.Exceptions;

namespace TransnationalLanka.ThreePL.WebApi.Util.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (ServiceException ex)
            {
                await HandleExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, new Exception($"Internal server error", ex));
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected
            var errorMessages = new List<ErrorMessage>();

            if (exception is ServiceException)
            {
                code = HttpStatusCode.BadRequest;
                var serviceException = ((ServiceException)(exception));
                errorMessages.AddRange(serviceException.Messages);
            }
            else
            {
                errorMessages.Add(new ErrorMessage()
                {
                    Code = null,
                    Message = exception.Message
                });
            }

            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                Errors = errorMessages
            }, new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            if (!context.Response.HasStarted)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)code;
            }

            return context.Response.WriteAsync(result);
        }
    }
}
