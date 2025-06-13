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

    public virtual DbSet<ListaEncuesta> ListaEncuesta { get; set; }

    public virtual DbSet<ListaEncuestados> ListaEncuestados { get; set; }

    public virtual DbSet<ListaPregunta> ListaPregunta { get; set; }

    public virtual DbSet<Pregunta> Pregunta { get; set; }

    public virtual DbSet<Realizada> Realizada { get; set; }

    public virtual DbSet<Respuestas> Respuestas { get; set; }

    public virtual DbSet<Usuario> Usuario { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;port=3306;user=root;password=kamisama;database=encuastas", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.4.0-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<ListaEncuesta>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("lista_encuesta");

            entity.HasIndex(e => e.IdListaPregunta, "Fk_ListaPregunta_idx");

            entity.HasIndex(e => e.IdRealizada, "Fk_Realizada_idx");

            entity.HasIndex(e => e.IdUsuario, "Fk_Usuario_idx");

            entity.Property(e => e.Activo).HasColumnType("blob");
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.IdListaPregunta).HasColumnName("Id_ListaPregunta");
            entity.Property(e => e.IdRealizada).HasColumnName("Id_Realizada");
            entity.Property(e => e.IdUsuario).HasColumnName("Id_Usuario");
            entity.Property(e => e.Nombre).HasMaxLength(100);

            entity.HasOne(d => d.IdListaPreguntaNavigation).WithMany(p => p.ListaEncuesta)
                .HasForeignKey(d => d.IdListaPregunta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Fk_ListaPregunta");

            entity.HasOne(d => d.IdRealizadaNavigation).WithMany(p => p.ListaEncuesta)
                .HasForeignKey(d => d.IdRealizada)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Fk_Realizada");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.ListaEncuesta)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Fk_Usuario");
        });

        modelBuilder.Entity<ListaEncuestados>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("lista_encuestados");

            entity.HasIndex(e => e.IdEncuestas, "Fk_Encuesta_idx");

            entity.HasIndex(e => e.IdListaPreguntas, "Fk_listapreguntas_idx");

            entity.HasIndex(e => e.IdUsuarios, "Fk_usuarios_idx");

            entity.Property(e => e.IdEncuestas).HasColumnName("Id_Encuestas");
            entity.Property(e => e.IdListaPreguntas).HasColumnName("Id_ListaPreguntas");
            entity.Property(e => e.IdUsuarios).HasColumnName("Id_usuarios");

            entity.HasOne(d => d.IdEncuestasNavigation).WithMany(p => p.ListaEncuestados)
                .HasForeignKey(d => d.IdEncuestas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Fk_Encuesta");

            entity.HasOne(d => d.IdListaPreguntasNavigation).WithMany(p => p.ListaEncuestados)
                .HasForeignKey(d => d.IdListaPreguntas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Fk_listapreguntas");

            entity.HasOne(d => d.IdUsuariosNavigation).WithMany(p => p.ListaEncuestados)
                .HasForeignKey(d => d.IdUsuarios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Fk_usuarios");
        });

        modelBuilder.Entity<ListaPregunta>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("lista_pregunta");

            entity.HasIndex(e => e.IdPregunta, "Fk_pregunta_idx");

            entity.Property(e => e.IdPregunta).HasColumnName("Id_pregunta");

            entity.HasOne(d => d.IdPreguntaNavigation).WithMany(p => p.ListaPregunta)
                .HasForeignKey(d => d.IdPregunta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Fk_pregunta");
        });

        modelBuilder.Entity<Pregunta>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("pregunta");

            entity.HasIndex(e => e.IdRespuesta, "FK_Respuesta_idx");

            entity.Property(e => e.IdRespuesta).HasColumnName("idRespuesta");
            entity.Property(e => e.Pregunta1)
                .HasMaxLength(5000)
                .HasColumnName("Pregunta");

            entity.HasOne(d => d.IdRespuestaNavigation).WithMany(p => p.Pregunta)
                .HasForeignKey(d => d.IdRespuesta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Respuesta");
        });

        modelBuilder.Entity<Realizada>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("realizada");

            entity.Property(e => e.Confirmar)
                .HasColumnType("blob")
                .HasColumnName("confirmar");
            entity.Property(e => e.Fecha).HasColumnType("datetime");
        });

        modelBuilder.Entity<Respuestas>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("respuestas");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Respuesta)
                .HasMaxLength(45)
                .HasColumnName("respuesta");
            entity.Property(e => e.Respuestacuatro)
                .HasMaxLength(45)
                .HasColumnName("respuestacuatro");
            entity.Property(e => e.Respuestados)
                .HasMaxLength(45)
                .HasColumnName("respuestados");
            entity.Property(e => e.Respuestatres)
                .HasMaxLength(45)
                .HasColumnName("respuestatres");
            entity.Property(e => e.Respuestauno)
                .HasMaxLength(45)
                .HasColumnName("respuestauno");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("usuario");

            entity.Property(e => e.Contrsena)
                .HasMaxLength(6)
                .HasColumnName("contrsena");
            entity.Property(e => e.Nombre).HasMaxLength(200);
            entity.Property(e => e.NumControl).HasMaxLength(8);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
