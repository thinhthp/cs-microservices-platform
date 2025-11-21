using SalesService.BLL.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesService.BLL.Grpcs.Product
{
    public interface IProductGrpcClient
    {
        Task<GetVariantResponse> GetVariantById(Guid id);
    }
}
