using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesService.Entities.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; }
        public DateTimeOffset PaidAt { get; set; }

        public Orders Order { get; set; }
    }
}
