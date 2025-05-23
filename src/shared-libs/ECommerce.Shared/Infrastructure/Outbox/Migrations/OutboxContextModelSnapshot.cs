﻿// <auto-generated />
using System;
using ECommerce.Shared.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ECommerce.Shared.Infrastructure.Outbox.Migrations
{
    [DbContext(typeof(OutboxContext))]
    partial class OutboxContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ECommerce.Shared.Infrastructure.Outbox.Models.OutboxEvent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("EventType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("Sent")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("OutboxEvents");
                });
#pragma warning restore 612, 618
        }
    }
}
