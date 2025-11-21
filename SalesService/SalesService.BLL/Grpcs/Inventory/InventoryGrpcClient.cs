using InventoryService.Grpc;
using SalesService.BLL.DTOs.Inventory;
using System;
using System.Threading;
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

        public async Task<GetDealerResponse?> GetDealerByIdAsync(Guid dealerId, CancellationToken ct = default)
        {
            var reply = await _client.GetDealerAsync(new GetDealerRequest { Id = dealerId.ToString() }, cancellationToken: ct);
            var dealer = reply?.Dealer;
            if (dealer is null) return null;

            if (!Guid.TryParse(dealer.Id, out var parsedId))
                return null;

            return new GetDealerResponse
            {
                Id = parsedId,
                Code = dealer.Code,
                Name = dealer.Name,
                Region = dealer.Region
            };
        }
    }
}