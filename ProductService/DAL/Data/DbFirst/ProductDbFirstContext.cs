using DAL.Entities.DbFirst;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data.DbFirst;

public partial class ProductDbFirstContext : DbContext
{
    public ProductDbFirstContext() { }

    public ProductDbFirstContext(DbContextOptions<ProductDbFirstContext> options)
        : base(options) { }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Model> Models { get; set; }

    public virtual DbSet<Variant> Variants { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        =>
        optionsBuilder.UseNpgsql(
            "User Id=postgres.hoojlnmnimgkogfimcbp;Password=admin@12345678;Server=aws-1-ap-southeast-1.pooler.supabase.com;Port=5432;Database=postgres"
        );

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("auth", "aal_level", new[] { "aal1", "aal2", "aal3" })
            .HasPostgresEnum("auth", "code_challenge_method", new[] { "s256", "plain" })
            .HasPostgresEnum("auth", "factor_status", new[] { "unverified", "verified" })
            .HasPostgresEnum("auth", "factor_type", new[] { "totp", "webauthn", "phone" })
            .HasPostgresEnum(
                "auth",
                "oauth_authorization_status",
                new[] { "pending", "approved", "denied", "expired" }
            )
            .HasPostgresEnum("auth", "oauth_client_type", new[] { "public", "confidential" })
            .HasPostgresEnum("auth", "oauth_registration_type", new[] { "dynamic", "manual" })
            .HasPostgresEnum("auth", "oauth_response_type", new[] { "code" })
            .HasPostgresEnum(
                "auth",
                "one_time_token_type",
                new[]
                {
                    "confirmation_token",
                    "reauthentication_token",
                    "recovery_token",
                    "email_change_token_new",
                    "email_change_token_current",
                    "phone_change_token",
                }
            )
            .HasPostgresEnum(
                "realtime",
                "action",
                new[] { "INSERT", "UPDATE", "DELETE", "TRUNCATE", "ERROR" }
            )
            .HasPostgresEnum(
                "realtime",
                "equality_op",
                new[] { "eq", "neq", "lt", "lte", "gt", "gte", "in" }
            )
            .HasPostgresEnum("storage", "buckettype", new[] { "STANDARD", "ANALYTICS", "VECTOR" })
            .HasPostgresExtension("extensions", "pg_stat_statements")
            .HasPostgresExtension("extensions", "pgcrypto")
            .HasPostgresExtension("extensions", "uuid-ossp")
            .HasPostgresExtension("graphql", "pg_graphql")
            .HasPostgresExtension("vault", "supabase_vault");

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("brands_pkey");

            entity.ToTable("brands");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Model>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("models_pkey");

            entity.ToTable("models");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").HasColumnName("id");
            entity.Property(e => e.BrandId).HasColumnName("brand_id");
            entity.Property(e => e.Name).HasColumnName("name");

            entity
                .HasOne(d => d.Brand)
                .WithMany(p => p.Models)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("models_brand_id_fkey");
        });

        modelBuilder.Entity<Variant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("variants_pkey");

            entity.ToTable("variants");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").HasColumnName("id");
            entity.Property(e => e.BasePrice).HasColumnName("base_price");
            entity.Property(e => e.ModelId).HasColumnName("model_id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.RangeKm).HasColumnName("range_km");

            entity
                .HasOne(d => d.Model)
                .WithMany(p => p.Variants)
                .HasForeignKey(d => d.ModelId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("variants_model_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
