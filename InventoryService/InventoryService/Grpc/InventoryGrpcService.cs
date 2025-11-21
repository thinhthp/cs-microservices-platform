using BLL.Interfaces;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace InventoryService.Grpc
{
    public class InventoryGrpcService : InventoryGrpc.InventoryGrpcBase
    {
        private readonly IDealerService _dealerService;
        private readonly ILogger<InventoryGrpcService> _logger;

        public InventoryGrpcService(IDealerService dealerService, ILogger<InventoryGrpcService> logger)
        {
            _dealerService = dealerService;
            _logger = logger;
        }

        public override async Task<GetDealerReply> GetDealer(GetDealerRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.Id, out var dealerId))
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"Invalid dealer id '{request.Id ?? "<null>"}'."));

            var dealer = await _dealerService.GetByIdAsync(dealerId);
            if (dealer is null)
            {
                _logger.LogWarning("Dealer not found. Id={Id}", request.Id);
                throw new RpcException(new Status(StatusCode.NotFound, $"Dealer '{request.Id}' not found."));
            }

            _logger.LogInformation("Dealer found. Id={Id} Code={Code}", dealer.Id, dealer.Code);

            return new GetDealerReply
            {
                Dealer = new Dealer
                {
                    Id = dealer.Id.ToString(),
                    Code = dealer.Code,
                    Name = dealer.Name ?? string.Empty,
                    Region = dealer.Region ?? string.Empty
                }
            };
        }
    }
}