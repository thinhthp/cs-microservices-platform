namespace DAL.Entities.DbFirst;

public partial class Model
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public long? BrandId { get; set; }

    public virtual Brand? Brand { get; set; }

    public virtual ICollection<Variant> Variants { get; set; } = new List<Variant>();
}
