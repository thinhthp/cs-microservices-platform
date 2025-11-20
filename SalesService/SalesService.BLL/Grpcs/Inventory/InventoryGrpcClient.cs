using InventoryService.Grpc;
using SalesService.BLL.DTOs.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesService.BLL.Grpcs.Inventory
{
    public class InventoryGrpcClient : IInventoryGrpcClient
    {
        private readonly InventoryGrpc.InventoryGrpcClient _client;

        public InventoryGrpcClient(InventoryGrpc.InventoryGrpcClient client)
        {
            _client = client;
        }
        public async Task<GetDealerResponse> GetDealerById(long id)
        {
            var reply = await _client.GetDealerAsync(new GetDealerRequest { Id = id });

            var dealer = reply?.Dealer;
            if (dealer is null)
                return new GetDealerResponse();

            return new GetDealerResponse
            {
                Id = ConvertDealerId(dealer.Id),
                Code = dealer.Code,
                Name = dealer.Name,
                Region = dealer.Region
            };
        }

        private static Guid LongToGuid(long value)
        {
            Span<byte> bytes = stackalloc byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes);
            return new Guid(bytes);
        }

        private static Guid ConvertDealerId(string id)
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
