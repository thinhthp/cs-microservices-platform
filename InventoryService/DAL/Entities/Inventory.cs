using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class Inventory
{
    public Guid Id { get; set; }

    public Guid DealerId { get; set; }

    public Guid? VariantId { get; set; }

    public long? Quantity { get; set; }

    public virtual Dealer Dealer { get; set; } = null!;
}
