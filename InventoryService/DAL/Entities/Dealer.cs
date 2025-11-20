using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class Dealer
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;

    public string? Name { get; set; }

    public string? Region { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
}
