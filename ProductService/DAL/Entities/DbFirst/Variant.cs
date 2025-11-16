namespace DAL.Entities.DbFirst;

public partial class Variant
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public long? RangeKm { get; set; }

    public double? BasePrice { get; set; }

    public long? ModelId { get; set; }

    public virtual Model? Model { get; set; }
}
