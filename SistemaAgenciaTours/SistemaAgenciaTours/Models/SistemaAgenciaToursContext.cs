using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SistemaAgenciaTours.Models;

public partial class SistemaAgenciaToursContext : DbContext
{
    public SistemaAgenciaToursContext()
    {
    }

    public SistemaAgenciaToursContext(DbContextOptions<SistemaAgenciaToursContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Destino> Destinos { get; set; }

    public virtual DbSet<Pai> Pais { get; set; }

    public virtual DbSet<Tour> Tours { get; set; }

    public virtual DbSet<VistaTourConEstado> VistaTourConEstados { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-GBKGIUE\\SQLEXPRESS;Database=SistemaAgenciaTours;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Destino>(entity =>
        {
            entity.HasKey(e => e.DestinoId).HasName("PK__Destino__4A838EF67A1BF4E6");

            entity.ToTable("Destino");

            entity.Property(e => e.DestinoId).HasColumnName("DestinoID");
            entity.Property(e => e.NombreDestino)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PaisId).HasColumnName("PaisID");

            entity.HasOne(d => d.Pais).WithMany(p => p.Destinos)
                .HasForeignKey(d => d.PaisId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Destino_Pais");
        });

        modelBuilder.Entity<Pai>(entity =>
        {
            entity.HasKey(e => e.PaisId).HasName("PK__Pais__B501E1A52867EE97");

            entity.Property(e => e.PaisId).HasColumnName("PaisID");
            entity.Property(e => e.NombrePais)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Tour>(entity =>
        {
            entity.HasKey(e => e.TourId).HasName("PK__Tour__604CEA10822D958D");

            entity.ToTable("Tour");

            entity.Property(e => e.TourId).HasColumnName("TourID");
            entity.Property(e => e.DestinoId).HasColumnName("DestinoID");
            entity.Property(e => e.Itbis)
                .HasComputedColumnSql("([Precio]*(0.18))", true)
                .HasColumnType("numeric(13, 4)")
                .HasColumnName("ITBIS");
            entity.Property(e => e.NombreTour)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.PaisId).HasColumnName("PaisID");
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Destino).WithMany(p => p.Tours)
                .HasForeignKey(d => d.DestinoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tour_Destino");

            entity.HasOne(d => d.Pais).WithMany(p => p.Tours)
                .HasForeignKey(d => d.PaisId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tour_Pais");
        });

        modelBuilder.Entity<VistaTourConEstado>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vista_TourConEstado");

            entity.Property(e => e.DestinoId).HasColumnName("DestinoID");
            entity.Property(e => e.Estado)
                .HasMaxLength(7)
                .IsUnicode(false);
            entity.Property(e => e.FechaHoraFin).HasColumnType("datetime");
            entity.Property(e => e.Itbis)
                .HasColumnType("numeric(13, 4)")
                .HasColumnName("ITBIS");
            entity.Property(e => e.NombreDestino)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NombrePais)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NombreTour)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.PaisId).HasColumnName("PaisID");
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TourId).HasColumnName("TourID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
