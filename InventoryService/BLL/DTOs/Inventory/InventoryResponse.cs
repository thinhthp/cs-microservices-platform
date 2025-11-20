using System;

namespace BLL.DTOs.Inventory;

public class InventoryResponse
{
    public Guid Id { get; set; }

    public Guid DealerId { get; set; }

    public Guid? VariantId { get; set; }

    public long? Quantity { get; set; }
}
