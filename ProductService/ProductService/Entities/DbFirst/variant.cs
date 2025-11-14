using System;
using System.Collections.Generic;

namespace ProductService.Entities.DbFirst;

public partial class variant
{
    public long id { get; set; }

    public string name { get; set; } = null!;

    public long? range_km { get; set; }

    public double? base_price { get; set; }

    public long? model_id { get; set; }

    public virtual model? model { get; set; }
}
