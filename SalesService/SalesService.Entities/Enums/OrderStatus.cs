using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesService.Entities.Enums
{
    public enum OrderStatus
    {
        Pending = 0,
        Processing = 1,
        Paid = 2,
        Shipped = 3,
        Completed = 4,
        Cancelled = 5
    }
}
