namespace DAL.Entities;

public partial class Inventory
{
    public long Id { get; set; }

    public long DealerId { get; set; }

    public long? VariantId { get; set; }

    public long? Quantity { get; set; }

    public virtual Dealer Dealer { get; set; } = null!;
}
