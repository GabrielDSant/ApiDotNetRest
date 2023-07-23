using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Microsoft.EntityFrameworkCore;
using ApiCrudDotNet.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.OpenApi.Models;



public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    public void ConfigureServices(IServiceCollection services)
    {
        // Configurar a conexão com o MySQL
        var connectionString = Configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
        );

        // Configurar o logger para escrever em um arquivo com formato JSON
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(
                new JsonFormatter(),
                "C:/Users/Gabri/Desktop/Projetos/ApiCrudDotNet/logs/log.txt",
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true,
                fileSizeLimitBytes: 10485760,
                shared: true,
                flushToDiskInterval: TimeSpan.FromSeconds(1),
                encoding: System.Text.Encoding.UTF8
            )
            .CreateLogger();
        // ...

        // Configurar o Swagger
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Minha API", Version = "v1" });
        });

        services.AddControllers();

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // ...

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minha API V1"); });
        }

        // Configurar o middleware de logging
        app.Use(async (context, next) =>
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
            await next();

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
        });

        // ...

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
