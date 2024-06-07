using Grpc.Core;
using Grpc.Net.Client;
using ITI.GRPCLab.Server.Protos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using System.Net;
using static ITI.GRPCLab.Server.Protos.InventoryService;

namespace ITI.GRPCLab.Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private const string ApiKey = "AIzaSyD7Q6Q6-4";

        [HttpPost]
        public async Task<ActionResult> AddProduct(Product product)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7080",
                new GrpcChannelOptions { Credentials = ChannelCredentials.SecureSsl});

            var callCredentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                metadata.Add("x-api-key", ApiKey);
                return Task.CompletedTask;
            });

            var client = new InventoryServiceClient(channel);

            var IsExisted = await client.GetProductByIdAsync(new Id { Id_ = product.Id }
            ,new CallOptions(credentials: callCredentials));

            if (IsExisted.IsExistd == false)
            {
                var addedProduct = await client.AddProductAsync(product);
                return Ok(addedProduct);
            }
            
            var Productedited = await client.UpdateProductAsync(product); 
            return Ok(Productedited);
        }

        [HttpPost("addproducts")]
        public async Task<ActionResult> AddBulkProducts(List<ProductToAdd> productToAdds)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7080");
            var client = new InventoryServiceClient(channel);

            var call = client.AddBulkProducts();

            foreach (var product in productToAdds)
            {
                await call.RequestStream.WriteAsync(product);
                await Task.Delay(1000);
            }

            await call.RequestStream.CompleteAsync();

            var response = await call.ResponseAsync;

            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult> GetReport()
        {
            List<ProductToAdd> productToAdds = new List<ProductToAdd>();

            var channel = GrpcChannel.ForAddress("https://localhost:7080",
                new GrpcChannelOptions { Credentials = ChannelCredentials.SecureSsl });

            var callCredentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                metadata.Add("x-api-key", ApiKey);
                return Task.CompletedTask;
            });

            var client = new InventoryServiceClient(channel);

            var call = client.GetProductReport(new Google.Protobuf.WellKnownTypes.Empty(),new CallOptions(credentials: callCredentials));

            while(await call.ResponseStream.MoveNext(CancellationToken.None))
            {
                productToAdds.Add(call.ResponseStream.Current);
            }

            return Ok(productToAdds);
        }
    }
}
