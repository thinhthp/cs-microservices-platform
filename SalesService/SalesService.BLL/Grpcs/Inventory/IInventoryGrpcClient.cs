using SalesService.BLL.DTOs.Inventory;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SalesService.BLL.Grpcs.Inventory
{
    public interface IInventoryGrpcClient
    {
        Task<GetDealerResponse?> GetDealerByIdAsync(Guid id, CancellationToken ct = default);
    }
}