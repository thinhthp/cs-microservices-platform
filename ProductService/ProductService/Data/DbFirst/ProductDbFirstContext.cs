using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ProductService.Entities.DbFirst;

namespace ProductService.Data.DbFirst;

public partial class ProductDbFirstContext : DbContext
{
    public ProductDbFirstContext(DbContextOptions<ProductDbFirstContext> options)
        : base(options)
    {
    }

    public virtual DbSet<brand> brands { get; set; }

    public virtual DbSet<model> models { get; set; }

    public virtual DbSet<variant> variants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("auth", "aal_level", new[] { "aal1", "aal2", "aal3" })
            .HasPostgresEnum("auth", "code_challenge_method", new[] { "s256", "plain" })
            .HasPostgresEnum("auth", "factor_status", new[] { "unverified", "verified" })
            .HasPostgresEnum("auth", "factor_type", new[] { "totp", "webauthn", "phone" })
            .HasPostgresEnum("auth", "oauth_authorization_status", new[] { "pending", "approved", "denied", "expired" })
            .HasPostgresEnum("auth", "oauth_client_type", new[] { "public", "confidential" })
            .HasPostgresEnum("auth", "oauth_registration_type", new[] { "dynamic", "manual" })
            .HasPostgresEnum("auth", "oauth_response_type", new[] { "code" })
            .HasPostgresEnum("auth", "one_time_token_type", new[] { "confirmation_token", "reauthentication_token", "recovery_token", "email_change_token_new", "email_change_token_current", "phone_change_token" })
            .HasPostgresEnum("realtime", "action", new[] { "INSERT", "UPDATE", "DELETE", "TRUNCATE", "ERROR" })
            .HasPostgresEnum("realtime", "equality_op", new[] { "eq", "neq", "lt", "lte", "gt", "gte", "in" })
            .HasPostgresEnum("storage", "buckettype", new[] { "STANDARD", "ANALYTICS" })
            .HasPostgresExtension("extensions", "pg_stat_statements")
            .HasPostgresExtension("extensions", "pgcrypto")
            .HasPostgresExtension("extensions", "uuid-ossp")
            .HasPostgresExtension("graphql", "pg_graphql")
            .HasPostgresExtension("vault", "supabase_vault");

        modelBuilder.Entity<brand>(entity =>
        {
            entity.HasKey(e => e.id).HasName("brands_pkey");
        });

        modelBuilder.Entity<model>(entity =>
        {
            entity.HasKey(e => e.id).HasName("models_pkey");

            entity.HasOne(d => d.brand).WithMany(p => p.models)
                .HasForeignKey(d => d.brand_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("models_brand_id_fkey");
        });

        modelBuilder.Entity<variant>(entity =>
        {
            entity.HasKey(e => e.id).HasName("variants_pkey");

            entity.HasOne(d => d.model).WithMany(p => p.variants)
                .HasForeignKey(d => d.model_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("variants_model_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
