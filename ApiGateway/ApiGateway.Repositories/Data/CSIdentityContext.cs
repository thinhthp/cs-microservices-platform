using ApiGateway.Entities.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGateway.Repositories.Data
{
    public class CSIdentityContext : IdentityDbContext<ApplicationUser>
    {
        public CSIdentityContext(DbContextOptions<CSIdentityContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.HasPostgresExtension("vector");

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("application_user");

                entity.Property(e => e.Address).HasColumnName("address");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP") // equivalent to now()
                    .HasColumnType("timestamptz")            // store as UTC
                    .HasColumnName("created_at");
                entity.Property(e => e.UpdateDate)
                    .HasColumnType("timestamptz")
                    .HasColumnName("update_date");
                entity.Property(e => e.CreateBy)
                    .HasMaxLength(40)
                    .HasColumnName("create_by");
                entity.Property(e => e.UpdateBy)
                    .HasMaxLength(40)
                    .HasColumnName("update_by");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.Note)
                    .HasMaxLength(255)
                    .HasColumnName("note");
            });
        }
    }
}
