namespace BLL.DTOs.Model;

public class ModelResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public long? BrandId { get; set; }
}
