using System;
using System.Collections.Generic;

namespace DAL.Entities.DbFirst;

public partial class Model
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid? BrandId { get; set; }

    public virtual Brand? Brand { get; set; }

    public virtual ICollection<Variant> Variants { get; set; } = new List<Variant>();
}
