using BLL.Interfaces;
using Grpc.Core;

namespace InventoryService.Grpc
{
    public class InventoryGrpcService : InventoryGrpc.InventoryGrpcBase
    {
        private readonly IDealerService _dealerService;

        public InventoryGrpcService(IDealerService dealerService)
        {
            _dealerService = dealerService;
        }

        public override async Task<GetDealerReply> GetDealer(GetDealerRequest request, ServerCallContext context)
        {
            var dealerDto = await _dealerService.GetByIdAsync(request.Id);
            if (dealerDto is null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Dealer '{request.Id}' not found."));

            return new GetDealerReply
            {
                Dealer = new Dealer
                {
                    Id = dealerDto.Id,
                    Code = dealerDto.Code,
                    Name = dealerDto.Name ?? string.Empty,
                    Region = dealerDto.Region ?? string.Empty
                }
            };
        }
    }
}