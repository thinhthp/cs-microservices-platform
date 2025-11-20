namespace BLL.DTOs.Model;

public class ModelResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? BrandId { get; set; }
}
