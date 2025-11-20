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
                Id = dealer.Id,
                Code = dealer.Code,
                Name = dealer.Name,
                Region = dealer.Region
            };
        }
    }
}
