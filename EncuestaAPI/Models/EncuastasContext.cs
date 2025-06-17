using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace EncuestaAPI.Models;

public partial class EncuastasContext : DbContext
{
    public EncuastasContext()
    {
    }

    public EncuastasContext(DbContextOptions<EncuastasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aplicacionencuesta> Aplicacionencuesta { get; set; }

    public virtual DbSet<Detallerespuesta> Detallerespuesta { get; set; }

    public virtual DbSet<Encuesta> Encuesta { get; set; }

    public virtual DbSet<Pregunta> Pregunta { get; set; }

    public virtual DbSet<Respuesta> Respuesta { get; set; }

    public virtual DbSet<Usuario> Usuario { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;port=3306;user=root;password=root;database=encuastas", Microsoft.EntityFrameworkCore.ServerVersion.Parse("9.3.0-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Aplicacionencuesta>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("aplicacionencuesta")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.IdEncuesta, "IdEncuesta");

            entity.HasIndex(e => e.IdUsuario, "IdUsuario");

            entity.Property(e => e.FechaFin).HasColumnType("datetime");
            entity.Property(e => e.FechaInicio)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdEncuestaNavigation).WithMany(p => p.Aplicacionencuesta)
                .HasForeignKey(d => d.IdEncuesta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("aplicacionencuesta_ibfk_2");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Aplicacionencuesta)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("aplicacionencuesta_ibfk_1");
        });

        modelBuilder.Entity<Detallerespuesta>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("detallerespuesta")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.IdPregunta, "IdPregunta");

            entity.HasIndex(e => e.IdRespuesta, "IdRespuesta");

            entity.HasOne(d => d.IdPreguntaNavigation).WithMany(p => p.Detallerespuesta)
                .HasForeignKey(d => d.IdPregunta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("detallerespuesta_ibfk_2");

            entity.HasOne(d => d.IdRespuestaNavigation).WithMany(p => p.Detallerespuesta)
                .HasForeignKey(d => d.IdRespuesta)
                .HasConstraintName("detallerespuesta_ibfk_1");
        });

        modelBuilder.Entity<Encuesta>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("encuesta")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.IdUsuario, "IdUsuario");

            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Nombre).HasMaxLength(100);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Encuesta)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("encuesta_ibfk_1");
        });

        modelBuilder.Entity<Pregunta>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("pregunta")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.IdEncuesta, "IdEncuesta");

            entity.Property(e => e.Texto).HasMaxLength(500);

            entity.HasOne(d => d.IdEncuestaNavigation).WithMany(p => p.Pregunta)
                .HasForeignKey(d => d.IdEncuesta)
                .HasConstraintName("pregunta_ibfk_1");
        });

        modelBuilder.Entity<Respuesta>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("respuesta")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.IdAplicacion, "IdAplicacion");

            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.NombreAlumno).HasMaxLength(100);
            entity.Property(e => e.NumControlAlumno).HasMaxLength(20);

            entity.HasOne(d => d.IdAplicacionNavigation).WithMany(p => p.Respuesta)
                .HasForeignKey(d => d.IdAplicacion)
                .HasConstraintName("respuesta_ibfk_1");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("usuario")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.NumControl, "numControl").IsUnique();

            entity.Property(e => e.Contraseña).HasMaxLength(255);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.NumControl)
                .HasMaxLength(8)
                .HasColumnName("numControl");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
