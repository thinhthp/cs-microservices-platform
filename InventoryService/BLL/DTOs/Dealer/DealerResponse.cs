using System;

namespace BLL.DTOs.Dealer;

public class DealerResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string? Name { get; set; }
    public string Region { get; set; } = null!;
}
