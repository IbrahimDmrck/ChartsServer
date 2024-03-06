
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ChartsServer.Models;

public partial class SatisDbContext : DbContext
{
    public SatisDbContext()
    {
    }

    public SatisDbContext(DbContextOptions<SatisDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Personeller> Personellers { get; set; }

    public virtual DbSet<Satislar> Satislars { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=.;Database=SatisDB;Trusted_Connection=True;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Personeller>(entity =>
        {
            entity.ToTable("Personeller");

            entity.Property(e => e.Adi).HasMaxLength(50);
            entity.Property(e => e.Soyadi).HasMaxLength(50);
        });

        modelBuilder.Entity<Satislar>(entity =>
        {
            entity.ToTable("Satislar");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
