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
                Id = ConvertVariantId(v.Id),
                Name = v.Name,
                RangeKm = v.RangeKm == 0 ? null : (long?)v.RangeKm,
                BasePrice = v.BasePrice == 0 ? null : (double?)v.BasePrice,
                ModelId = ConvertVariantId(v.ModelId.ToString())
            };
        }

        private static Guid LongToGuid(long value)
        {
            Span<byte> bytes = stackalloc byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes);
            return new Guid(bytes);
        }

        private static Guid ConvertVariantId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Guid.Empty;

            if (Guid.TryParse(id, out var guid))
                return guid;

            if (long.TryParse(id, out var longValue))
                return LongToGuid(longValue);

            return Guid.Empty;
        }
    }
}