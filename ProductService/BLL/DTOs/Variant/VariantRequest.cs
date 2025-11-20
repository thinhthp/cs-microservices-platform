namespace BLL.DTOs.Variant;

public class VariantRequest
{
    public string Name { get; set; } = string.Empty;
    public long? RangeKm { get; set; }
    public double? BasePrice { get; set; }
    public Guid? ModelId { get; set; }
}
