using System;
using System.Collections.Generic;

namespace ProductService.Entities.DbFirst;

public partial class model
{
    public long id { get; set; }

    public string name { get; set; } = null!;

    public long? brand_id { get; set; }

    public virtual brand? brand { get; set; }

    public virtual ICollection<variant> variants { get; set; } = new List<variant>();
}
