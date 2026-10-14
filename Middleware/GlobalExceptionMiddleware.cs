using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TH5.Dtos.Common;
using TH5.Exceptions;

namespace TH5.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("Response da bat dau, khong the ghi exception ra client.");
                    throw;
                }
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            int statusCode;
            string message;

            if (exception is UserFriendlyException ufe)
            {
                statusCode = ufe.StatusCode;
                message = ufe.Message;
                _logger.LogWarning("Lỗi nghiệp vụ ({StatusCode}): {Message}", statusCode, message);
            }
            else
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                message = "Đã xảy ra lỗi không mong muốn trên hệ thống.";
                _logger.LogError(exception, "Lỗi chưa được xử lý: {Message}", exception.Message);
            }

            context.Response.StatusCode = statusCode;

            var response = ApiResponse<object>.Failure(message, statusCode);
            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}
