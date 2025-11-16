namespace BLL.DTOs.Variant;

public class VariantResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public long? RangeKm { get; set; }
    public double? BasePrice { get; set; }
    public long? ModelId { get; set; }
}
