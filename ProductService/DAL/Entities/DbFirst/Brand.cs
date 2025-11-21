using System;
using System.Collections.Generic;

namespace DAL.Entities.DbFirst;

public partial class Brand
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Model> Models { get; set; } = new List<Model>();
}
