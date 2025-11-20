using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesService.BLL.DTOs.Inventory
{
    public sealed class GetDealerResponse
    {
        public Guid Id { get; set; }

        public string Code { get; set; } = null!;

        public string? Name { get; set; }

        public string Region { get; set; } = null!;
    }
}
