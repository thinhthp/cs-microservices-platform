using System;
using System.Collections.Generic;

namespace ProductService.Entities.DbFirst;

public partial class brand
{
    public long id { get; set; }

    public string name { get; set; } = null!;

    public virtual ICollection<model> models { get; set; } = new List<model>();
}
