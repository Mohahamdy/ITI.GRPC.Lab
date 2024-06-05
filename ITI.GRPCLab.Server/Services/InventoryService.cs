using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ITI.GRPCLab.Server.Protos;
using Microsoft.AspNetCore.Authorization;
using static ITI.GRPCLab.Server.Protos.InventoryService;

namespace ITI.GRPCLab.Server.Services
{
    public class InventoryService : InventoryServiceBase
    {
        
        public List<Product> Products { get; set; }
        public List<ProductToAdd> ProductsToAdd { get; set; } = new List<ProductToAdd>();
          

        public InventoryService()
        {
            Products = new List<Product>()
            {
                new Product{Id=1,Name="P1",Descripton="Best",Quantity=10},
                new Product{Id=2,Name="P2",Descripton="Best",Quantity=10},
                new Product{Id=3,Name="P3",Descripton="Best",Quantity=10}
            };

            ProductsToAdd = new List<ProductToAdd>()
            {
                new ProductToAdd{Id = 1,Name="P1",Quantity = 5,Category = Category.Laptops,ExpiredDate = Timestamp.FromDateTime(DateTime.UtcNow) },
                new ProductToAdd{Id = 2,Name="P2",Quantity = 5,Category = Category.Laptops,ExpiredDate = Timestamp.FromDateTime(DateTime.UtcNow) },
                new ProductToAdd{Id = 3,Name="P3",Quantity = 5,Category = Category.Laptops,ExpiredDate = Timestamp.FromDateTime(DateTime.UtcNow) },
                new ProductToAdd{Id = 4,Name="P4",Quantity = 5,Category = Category.Laptops,ExpiredDate = Timestamp.FromDateTime(DateTime.UtcNow) }
            };
        }

        [Authorize(AuthenticationSchemes = Consts.ApiKeySchemeName)]
        public override async Task<IsProductExisted> GetProductById(Id request, ServerCallContext context)
        {
            var prodcut = Products.FirstOrDefault(p => p.Id == request.Id_);

            if (prodcut != null)
            {
                return await Task.FromResult(new IsProductExisted
                {
                    IsExistd = true,
                });
            }

            return await Task.FromResult(new IsProductExisted
            {
                IsExistd = false,
            });
        }
       
        [Authorize(AuthenticationSchemes = Consts.ApiKeySchemeName)]
        public override async Task<Product> AddProduct(Product request, ServerCallContext context)
        {
            Products.Add(request);

            return await Task.FromResult(request);
        }
       
        [Authorize(AuthenticationSchemes = Consts.ApiKeySchemeName)]
        public override async Task<Product> UpdateProduct(Product request, ServerCallContext context)
        {
            var product = Products.FirstOrDefault(p => p.Id == request.Id);

            product.Name = request.Name;
            product.Descripton = request.Descripton;
            product.Quantity = request.Quantity;

            return await Task.FromResult(product);
        }
       
        [Authorize(AuthenticationSchemes = Consts.ApiKeySchemeName)]
        public override async Task<NumberOfInsertedProducts> AddBulkProducts(IAsyncStreamReader<ProductToAdd> requestStream, ServerCallContext context)
        {
            int count = 0;
            await foreach (var request in requestStream.ReadAllAsync())
            {
                ProductsToAdd.Add(request);
                ++count;
            }

            return await Task.FromResult(new NumberOfInsertedProducts { Count = count});
        }
       
        [Authorize(AuthenticationSchemes = Consts.ApiKeySchemeName)]
        public override async Task GetProductReport(Empty request, IServerStreamWriter<ProductToAdd> responseStream, ServerCallContext context)
        {
            foreach (var item in ProductsToAdd)
            {
                await responseStream.WriteAsync(item);
            }

            await Task.CompletedTask;
        }
    }
}
