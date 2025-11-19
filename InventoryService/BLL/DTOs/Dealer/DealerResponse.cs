namespace BLL.DTOs.Dealer;

public class DealerResponse
{
    public long Id { get; set; }
    public string Code { get; set; } = null!;
    public string? Name { get; set; }
    public string Region { get; set; } = null!;
}
