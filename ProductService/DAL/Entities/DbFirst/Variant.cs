using System;
using System.Collections.Generic;

namespace DAL.Entities.DbFirst;

public partial class Variant
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public long? RangeKm { get; set; }

    public double? BasePrice { get; set; }

    public Guid? ModelId { get; set; }

    public virtual Model? Model { get; set; }
}
