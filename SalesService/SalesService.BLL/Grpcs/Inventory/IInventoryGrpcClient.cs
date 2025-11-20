using SalesService.BLL.DTOs.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesService.BLL.Grpcs.Inventory
{
    public interface IInventoryGrpcClient
    {
        Task<GetDealerResponse> GetDealerById(long id);
    }
}
