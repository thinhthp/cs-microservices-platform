namespace BLL.DTOs.Inventory;

public class InventoryRequest
{
    public long DealerId { get; set; }

    public long? VariantId { get; set; }

    public long? Quantity { get; set; }
}
