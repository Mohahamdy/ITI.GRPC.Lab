
using ITI.GRPCLab.Client.Services;
using ITI.GRPCLab.Server;
using ITI.GRPCLab.Server.Protos;

namespace ITI.GRPCLab.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var _configuration = builder.Configuration;
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddScoped<IApiKeyProviderService, ApiKeyProviderService>();

            builder.Services.AddGrpcClient<InventoryService.InventoryServiceClient>(options =>
            {
                var address = _configuration.GetValue<string>(Consts.GrpcServiceAddressSettingName);
                options.Address = new Uri(address);
            }).AddCallCredentials((context, metadata, serviceProvider) =>
            {
                var apiKeyProvider = serviceProvider.GetRequiredService<IApiKeyProviderService>();
                var apiKey = apiKeyProvider.GetApiKey();
                metadata.Add(Consts.ApiKeyHeaderName, apiKey);
                return Task.CompletedTask;
            });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
