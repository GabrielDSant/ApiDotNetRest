using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using Serilog;

namespace ApiCrudDotNet.Middlewares
{

    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Armazenar a data e hora da requisição
            var requestTime = DateTime.UtcNow;

            // Armazenar o corpo da requisição
            string requestBody = "";
            if (context.Request.Method == "POST" || context.Request.Method == "PUT")
            {
                context.Request.EnableBuffering();
                var requestBodyStream = new StreamReader(context.Request.Body);
                requestBody = await requestBodyStream.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            // Chamar o próximo middleware na pipeline
            await _next(context);

            // Armazenar o corpo da resposta
            string responseBody = "";
            if (context.Response.StatusCode != StatusCodes.Status204NoContent) // Não há corpo na resposta 204 No Content
            {
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);
            }

            // Armazenar o IP de quem enviou a requisição
            string remoteIpAddress = context.Connection.RemoteIpAddress.ToString();

            // Registrar as informações no logger
            Log.Information(
                "Data e Hora: {RequestTime}, IP: {RemoteIpAddress}, Body da Requisição: {RequestBody}, Body da Resposta: {ResponseBody}",
                requestTime, remoteIpAddress, requestBody, responseBody);
        }
    }
}
