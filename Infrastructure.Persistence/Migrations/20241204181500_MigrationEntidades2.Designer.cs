﻿// <auto-generated />
using System;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(ApplicactionContext))]
    [Migration("20241204181500_MigrationEntidades2")]
    partial class MigrationEntidades2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Core.Domain.Entities.Reserva", b =>
                {
                    b.Property<int>("ReservaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ReservaId"));

                    b.Property<DateTime>("FechaFin")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("FechaInicio")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("PrecioTotal")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("UsuarioId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("VehiculoId")
                        .HasColumnType("int");

                    b.HasKey("ReservaId");

                    b.HasIndex("VehiculoId");

                    b.ToTable("Reserva", (string)null);
                });

            modelBuilder.Entity("Core.Domain.Entities.Vehiculo", b =>
                {
                    b.Property<int>("VehiculoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("VehiculoId"));

                    b.Property<int>("Año")
                        .HasColumnType("int");

                    b.Property<string>("Categoria")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Combustible")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Descripcion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Estado")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImagenVehiculo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Marca")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Modelo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NumeroPersonas")
                        .HasColumnType("int");

                    b.Property<int>("PrecioPorDia")
                        .HasColumnType("int");

                    b.Property<string>("Transmision")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("VehiculoId");

                    b.ToTable("Vehiculo", (string)null);
                });

            modelBuilder.Entity("Core.Domain.Entities.Reserva", b =>
                {
                    b.HasOne("Core.Domain.Entities.Vehiculo", "Vehiculo")
                        .WithMany("Reservas")
                        .HasForeignKey("VehiculoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Vehiculo");
                });

            modelBuilder.Entity("Core.Domain.Entities.Vehiculo", b =>
                {
                    b.Navigation("Reservas");
                });
#pragma warning restore 612, 618
        }
    }
}
