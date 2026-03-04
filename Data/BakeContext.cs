using System;
using System.Collections.Generic;
using Bake.Models;
using Microsoft.EntityFrameworkCore;

namespace Bake.Data;

public partial class BakeContext : DbContext
{
    public BakeContext()
    {
    }

    public BakeContext(DbContextOptions<BakeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:bake.database.windows.net,1433;Initial Catalog=bake;Persist Security Info=False;User ID=bake;Password=tjm103_01;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK_Prodeuct_Category");

            entity.ToTable("Product_Category", "Sales");

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryDescription)
                .HasMaxLength(200)
                .HasColumnName("category_description");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(50)
                .HasColumnName("category_name");
            entity.Property(e => e.DisplayOrder).HasColumnName("display_order");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
