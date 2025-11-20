using InventoryService.Grpc;
using ProductService.Grpc;
using SalesService.BLL.DTOs.Inventory;
using SalesService.BLL.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesService.BLL.Grpcs.Product
{
    public class ProductGrpcClient : IProductGrpcClient
    {
        private readonly ProductGrpc.ProductGrpcClient _client;

        public ProductGrpcClient(ProductGrpc.ProductGrpcClient client)
        {
            _client = client;
        }

        public async Task<GetVariantResponse> GetVariantById(long id)
        {
            var reply = await _client.GetVariantAsync(new GetVariantRequest { Id = id });

            var v = reply?.Variant;
            if (v is null)
                return new GetVariantResponse();

            return new GetVariantResponse
            {
                Id = v.Id,
                Name = v.Name,
                RangeKm = v.RangeKm == 0 ? null : (long?)v.RangeKm,
                BasePrice = v.BasePrice == 0 ? null : (double?)v.BasePrice,
                ModelId = v.ModelId == 0 ? null : (long?)v.ModelId
            };
        }
    }
}