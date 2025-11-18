namespace BLL.DTOs.Inventory;

public class InventoryResponse
{
    public long Id { get; set; }

    public long DealerId { get; set; }

    public long? VariantId { get; set; }

    public long? Quantity { get; set; }
}
