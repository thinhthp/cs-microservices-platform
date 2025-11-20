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
            var dealerDto = await _dealerService.GetByIdAsync(LongToGuid(request.Id));
            if (dealerDto is null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Dealer '{request.Id}' not found."));

            return new GetDealerReply
            {
                Dealer = new Dealer
                {
                    Id = dealerDto.Id.ToString(),
                    Code = dealerDto.Code,
                    Name = dealerDto.Name ?? string.Empty,
                    Region = dealerDto.Region ?? string.Empty
                }
            };
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

        private static Guid LongToGuid(long value)
        {
            Span<byte> bytes = stackalloc byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes);
            return new Guid(bytes);
        }
    }
}