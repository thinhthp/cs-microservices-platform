using System;

namespace BLL.DTOs.Inventory;

public class InventoryRequest
{
    public Guid DealerId { get; set; }

    public Guid? VariantId { get; set; }

    public long? Quantity { get; set; }
}
