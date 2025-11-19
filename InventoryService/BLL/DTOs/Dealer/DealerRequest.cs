namespace BLL.DTOs.Dealer;

public class DealerRequest
{
    public string Code { get; set; } = null!;
    public string? Name { get; set; }
    public string Region { get; set; } = null!;
}
