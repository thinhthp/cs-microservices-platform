using System.Threading.Tasks;
using Grpc.Core;
using BLL.Interfaces;

namespace ProductService.Grpc
{
    public sealed class ProductGrpcService : ProductGrpc.ProductGrpcBase
    {
        private readonly IVariantService _variantService;

        public ProductGrpcService(IVariantService variantService)
        {
            _variantService = variantService;
        }

        public override async Task<GetVariantReply> GetVariant(GetVariantRequest request, ServerCallContext context)
        {
            if (request is null || request.Id <= 0)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Variant id must be positive."));

            var variantDto = await _variantService.GetByIdAsync(request.Id);
            if (variantDto is null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Variant {request.Id} not found."));

            var reply = new GetVariantReply
            {
                Variant = new Variant
                {
                    Id = variantDto.Id,
                    Name = variantDto.Name,
                    RangeKm = variantDto.RangeKm ?? 0,
                    BasePrice = variantDto.BasePrice ?? 0,
                    ModelId = variantDto.ModelId ?? 0
                }
            };

            return reply;
        }
    }
}