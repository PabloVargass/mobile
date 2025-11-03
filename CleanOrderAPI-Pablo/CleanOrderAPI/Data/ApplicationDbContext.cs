using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using CleanOrderAPI.Data.Entities;

namespace CleanOrderAPI.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cliente> Clientes { get; set; }
    public virtual DbSet<Comuna> Comunas { get; set; }
    public virtual DbSet<Documento> Documentos { get; set; }
    public virtual DbSet<Empleado> Empleados { get; set; }
    public virtual DbSet<ImagenesReporte> ImagenesReportes { get; set; }
    public virtual DbSet<Orden> Ordens { get; set; }
    public virtual DbSet<OrdenEmpleado> OrdenEmpleados { get; set; }
    public virtual DbSet<OrdenEstado> OrdenEstados { get; set; }
    public virtual DbSet<Region> Regions { get; set; }
    public virtual DbSet<Reporte> Reportes { get; set; }
    public virtual DbSet<Rol> Rols { get; set; }
    public virtual DbSet<TipoCarga> TipoCargas { get; set; }
    public virtual DbSet<Usuario> Usuarios { get; set; }
    public virtual DbSet<Vehiculo> Vehiculos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.RutCliente).HasName("PRIMARY");

            entity.ToTable("cliente");

            entity.Property(e => e.RutCliente)
                .HasMaxLength(10)
                .HasColumnName("RUT_CLIENTE");
            entity.Property(e => e.Activo)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasColumnName("ACTIVO");
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .HasColumnName("CORREO");
            entity.Property(e => e.Dv)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasColumnName("DV");
            entity.Property(e => e.RazonSocial)
                .HasMaxLength(60)
                .HasColumnName("RAZON_SOCIAL");
            entity.Property(e => e.Telefono)
                .HasMaxLength(12)
                .HasColumnName("TELEFONO");
        });

        modelBuilder.Entity<Comuna>(entity =>
        {
            entity.HasKey(e => e.Codigo).HasName("PRIMARY");

            entity.ToTable("comuna");

            entity.HasIndex(e => e.FkCodigoRegion, "REGION_COMUNA");

            entity.Property(e => e.Codigo)
                .ValueGeneratedNever()
                .HasColumnType("int(11)")
                .HasColumnName("CODIGO");
            entity.Property(e => e.FkCodigoRegion)
                .HasColumnType("int(11)")
                .HasColumnName("FK_CODIGO_REGION");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("NOMBRE");

            entity.HasOne(d => d.FkCodigoRegionNavigation).WithMany(p => p.Comunas)
                .HasForeignKey(d => d.FkCodigoRegion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("REGION_COMUNA");
        });

        modelBuilder.Entity<Documento>(entity =>
        {
            entity.HasKey(e => e.IdDocumento).HasName("PRIMARY");

            entity.ToTable("documento");

            entity.HasIndex(e => e.FkRutCliente, "DOCUMENTOS_CLIENTES");

            entity.Property(e => e.IdDocumento)
                .HasColumnType("int(11)")
                .ValueGeneratedOnAdd()
                .HasColumnName("ID_DOCUMENTO");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("NOMBRE");
            entity.Property(e => e.FechaSubida)
                .HasColumnType("datetime")
                .HasColumnName("FECHA_SUBIDA");
            entity.Property(e => e.TipoMime)
                .HasMaxLength(50)
                .HasColumnName("TIPO_MIME");
            entity.Property(e => e.Archivo)
                .HasColumnType("LONGBLOB")
                .HasColumnName("ARCHIVO");
            entity.Property(e => e.TamanoBytes)
                .HasColumnType("int(11)")
                .HasColumnName("TAMANO_BYTES");
            entity.Property(e => e.FkRutCliente)
                .HasMaxLength(10)
                .HasColumnName("FK_RUT_CLIENTE");

            entity.HasOne(d => d.FkRutClienteNavigation).WithMany(p => p.Documentos)
                .HasForeignKey(d => d.FkRutCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("DOCUMENTOS_CLIENTES");
        });

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(e => e.RutEmpleado).HasName("PRIMARY");

            entity.ToTable("empleado");

            entity.HasIndex(e => e.FkComuna, "EMPLEADOS_COMUNA");

            entity.Property(e => e.RutEmpleado)
                .HasMaxLength(10)
                .HasColumnName("RUT_EMPLEADO");
            entity.Property(e => e.Activo)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasColumnName("ACTIVO");
            entity.Property(e => e.Apellido)
                .HasMaxLength(50)
                .HasColumnName("APELLIDO");
            entity.Property(e => e.Direccion)
                .HasMaxLength(50)
                .HasColumnName("DIRECCION");
            entity.Property(e => e.Dv)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasColumnName("DV");
            entity.Property(e => e.FkComuna)
                .HasColumnType("int(11)")
                .HasColumnName("FK_COMUNA");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("NOMBRE");
            entity.Property(e => e.Telefono)
                .HasMaxLength(12)
                .HasColumnName("TELEFONO");

            entity.HasOne(d => d.FkComunaNavigation).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.FkComuna)
                .HasConstraintName("EMPLEADOS_COMUNA");
        });

        modelBuilder.Entity<ImagenesReporte>(entity =>
        {
            entity.HasKey(e => e.IdImgReporte).HasName("PRIMARY");

            entity.ToTable("imagenes_reporte");

            entity.HasIndex(e => e.FkIdReporte, "REPORTE_IMAGENES");

            entity.Property(e => e.IdImgReporte)
                .HasColumnType("int(11)")
                .HasColumnName("ID_IMG_REPORTE");
            entity.Property(e => e.Archivo).HasColumnName("ARCHIVO");
            entity.Property(e => e.FkIdReporte)
                .HasColumnType("int(11)")
                .HasColumnName("FK_ID_REPORTE");
            entity.Property(e => e.TipoMime)
                .HasMaxLength(50)
                .HasColumnName("TIPO_MIME");

            entity.HasOne(d => d.FkIdReporteNavigation).WithMany(p => p.ImagenesReportes)
                .HasForeignKey(d => d.FkIdReporte)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("REPORTE_IMAGENES");
        });

        modelBuilder.Entity<Orden>(entity =>
        {
            entity.HasKey(e => e.IdOrden).HasName("PRIMARY");

            entity.ToTable("orden");

            entity.HasIndex(e => e.Folio, "FOLIO").IsUnique();
            entity.HasIndex(e => e.FkRutClientes, "ORDENES_CLIENTES");
            entity.HasIndex(e => e.FkComuna, "ORDENES_COMUNA");
            entity.HasIndex(e => e.FkPatente, "ORDENES_VEHICULOS");
            entity.HasIndex(e => e.FkEstado, "ORDEN_ESTADO");

            entity.Property(e => e.IdOrden)
                .HasColumnType("int(11)")
                .HasColumnName("ID_ORDEN");
            entity.Property(e => e.Direccion)
                .HasMaxLength(50)
                .HasColumnName("DIRECCION");
            entity.Property(e => e.FechaAgendada)
                .HasColumnType("datetime")
                .HasColumnName("FECHA_AGENDADA");
            entity.Property(e => e.FechaRegistro)
                .HasColumnType("datetime")
                .HasColumnName("FECHA_REGISTRO");
            entity.Property(e => e.FkComuna)
                .HasColumnType("int(11)")
                .HasColumnName("FK_COMUNA");
            entity.Property(e => e.FechaFinalizado)
                .HasColumnType("datetime")
                .HasColumnName("FECHA_FINALIZADO");
            entity.Property(e => e.FkEstado)
                .HasColumnType("int(11)")
                .HasColumnName("FK_ESTADO");
            entity.Property(e => e.FkPatente)
                .HasMaxLength(8)
                .HasColumnName("FK_PATENTE");
            entity.Property(e => e.FkRutClientes)
                .HasMaxLength(10)
                .HasColumnName("FK_RUT_CLIENTES");
            entity.Property(e => e.Folio)
                .HasColumnType("int(11)")
                .HasColumnName("FOLIO");
            entity.Property(e => e.HorasTrabajo)
                .HasColumnType("int(11)")
                .HasColumnName("HORAS_TRABAJO");
            entity.Property(e => e.Observacion)
                .HasMaxLength(100)
                .HasColumnName("OBSERVACION");

            entity.HasOne(d => d.FkComunaNavigation).WithMany(p => p.Ordens)
                .HasForeignKey(d => d.FkComuna)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ORDENES_COMUNA");

            entity.HasOne(d => d.FkEstadoNavigation).WithMany(p => p.Ordens)
                .HasForeignKey(d => d.FkEstado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ORDEN_ESTADO");

            entity.HasOne(d => d.FkPatenteNavigation).WithMany(p => p.Ordens)
                .HasForeignKey(d => d.FkPatente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ORDENES_VEHICULOS");

            entity.HasOne(d => d.FkRutClientesNavigation).WithMany(p => p.Ordens)
                .HasForeignKey(d => d.FkRutClientes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ORDENES_CLIENTES");
        });

        // FIX: configure composite key for the join table so EF Core can track/add/remove rows
        modelBuilder.Entity<OrdenEmpleado>(entity =>
        {
            entity.ToTable("orden_empleado");

            entity.HasKey(e => new { e.FkIdOrdenes, e.FkRutEmpleado }).HasName("PK_orden_empleado");

            entity.HasIndex(e => e.FkRutEmpleado, "OxE_EMPLEADOS");
            entity.HasIndex(e => e.FkIdOrdenes, "OxE_ORDENES");

            entity.Property(e => e.FkIdOrdenes)
                .HasColumnType("int(11)")
                .HasColumnName("FK_ID_ORDENES");
            entity.Property(e => e.FkRutEmpleado)
                .HasMaxLength(10)
                .HasColumnName("FK_RUT_EMPLEADO");

            entity.HasOne(d => d.FkIdOrdenesNavigation).WithMany()
                .HasForeignKey(d => d.FkIdOrdenes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("OxE_ORDENES");

            entity.HasOne(d => d.FkRutEmpleadoNavigation).WithMany()
                .HasForeignKey(d => d.FkRutEmpleado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("OxE_EMPLEADOS");
        });

        modelBuilder.Entity<OrdenEstado>(entity =>
        {
            entity.HasKey(e => e.IdEstado).HasName("PRIMARY");

            entity.ToTable("orden_estado");

            entity.Property(e => e.IdEstado)
                .HasColumnType("int(11)")
                .HasColumnName("ID_ESTADO");
            entity.Property(e => e.Nombre)
                .HasMaxLength(30)
                .HasColumnName("NOMBRE");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.Codigo).HasName("PRIMARY");

            entity.ToTable("region");

            entity.Property(e => e.Codigo)
                .ValueGeneratedNever()
                .HasColumnType("int(11)")
                .HasColumnName("CODIGO");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("NOMBRE");
        });

        modelBuilder.Entity<Reporte>(entity =>
        {
            entity.HasKey(e => e.IdReporte).HasName("PRIMARY");

            entity.ToTable("reporte");

            entity.HasIndex(e => e.FkIdOrden, "ORDEN_REPORTE");

            entity.Property(e => e.IdReporte)
                .HasColumnType("int(11)")
                .HasColumnName("ID_REPORTE");
            entity.Property(e => e.FkIdOrden)
                .HasColumnType("int(11)")
                .HasColumnName("FK_ID_ORDEN");
            entity.Property(e => e.FkUsuario)
                .HasColumnType("int(11)")
                .HasColumnName("FK_USUARIO");
            entity.Property(e => e.Observacion)
                .HasMaxLength(100)
                .HasColumnName("OBSERVACION");

            entity.HasOne(d => d.FkIdOrdenNavigation).WithMany(p => p.Reportes)
                .HasForeignKey(d => d.FkIdOrden)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ORDEN_REPORTE");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PRIMARY");

            entity.ToTable("rol");

            entity.Property(e => e.IdRol)
                .HasColumnType("int(11)")
                .HasColumnName("ID_ROL");
            entity.Property(e => e.Nombre)
                .HasMaxLength(40)
                .HasColumnName("NOMBRE");
        });

        modelBuilder.Entity<TipoCarga>(entity =>
        {
            entity.HasKey(e => e.TipoCargaCodigo).HasName("PRIMARY");

            entity.ToTable("tipo_carga");

            entity.Property(e => e.TipoCargaCodigo)
                .HasColumnType("int(11)")
                .HasColumnName("TIPO_CARGA_CODIGO");
            entity.Property(e => e.NombreCarga)
                .HasMaxLength(50)
                .HasColumnName("NOMBRE_CARGA");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PRIMARY");

            entity.ToTable("usuario");

            entity.HasIndex(e => e.Correo, "CORREO").IsUnique();
            entity.HasIndex(e => e.FkRutEmpleado, "EMPLEADO_USUARIO");
            entity.HasIndex(e => e.FkIdRol, "USUARIO_ROL");

            entity.Property(e => e.IdUsuario)
                .HasColumnType("int(11)")
                .HasColumnName("ID_USUARIO");
            entity.Property(e => e.Activo)
                .HasColumnType("int(11)")
                .HasColumnName("ACTIVO");
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .HasColumnName("CORREO");
            entity.Property(e => e.FkIdRol)
                .HasColumnType("int(11)")
                .HasColumnName("FK_ID_ROL");
            entity.Property(e => e.FkRutEmpleado)
                .HasMaxLength(10)
                .HasColumnName("FK_RUT_EMPLEADO");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsFixedLength()
                .HasColumnName("PASSWORD");

            entity.HasOne(d => d.FkIdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.FkIdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("USUARIO_ROL");

            entity.HasOne(d => d.FkRutEmpleadoNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.FkRutEmpleado)
                .HasConstraintName("EMPLEADO_USUARIO");
        });

        modelBuilder.Entity<Vehiculo>(entity =>
        {
            entity.HasKey(e => e.Patente).HasName("PRIMARY");

            entity.ToTable("vehiculo");

            entity.HasIndex(e => e.FkTipo, "VEHICULO_TIPO_CARGA");

            entity.Property(e => e.Patente)
                .HasMaxLength(8)
                .HasColumnName("PATENTE");
            entity.Property(e => e.Activo)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasColumnName("ACTIVO");
            entity.Property(e => e.Capacidad)
                .HasColumnType("int(11)")
                .HasColumnName("CAPACIDAD");
            entity.Property(e => e.FkTipo)
                .HasColumnType("int(11)")
                .HasColumnName("FK_TIPO");

            entity.HasOne(d => d.FkTipoNavigation).WithMany(p => p.Vehiculos)
                .HasForeignKey(d => d.FkTipo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("VEHICULO_TIPO_CARGA");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
