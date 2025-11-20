using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesService.BLL.DTOs.Product
{
    public sealed class GetVariantResponse
    {
        public long Id { get; set; }

        public string Name { get; set; } = null!;

        public long? RangeKm { get; set; }

        public double? BasePrice { get; set; }

        public long? ModelId { get; set; }
    }
}
