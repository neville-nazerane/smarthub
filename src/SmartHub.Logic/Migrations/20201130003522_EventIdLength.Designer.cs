﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SmartHub.Logic.Data;

namespace SmartHub.Logic.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20201130003522_EventIdLength")]
    partial class EventIdLength
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("SmartHub.Models.Entities.DeviceAction", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("Capability")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4");

                    b.Property<string>("Command")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4");

                    b.Property<string>("Component")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4");

                    b.Property<string>("DeviceId")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("varchar(60) CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("DeviceActions");
                });

            modelBuilder.Entity("SmartHub.Models.Entities.EventLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("EventId")
                        .IsRequired()
                        .HasMaxLength(90)
                        .HasColumnType("varchar(90) CHARACTER SET utf8mb4");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.ToTable("EventLogs");
                });

            modelBuilder.Entity("SmartHub.Models.Entities.SceneAction", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("SceneId")
                        .IsRequired()
                        .HasMaxLength(90)
                        .HasColumnType("varchar(90) CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("SceneActions");
                });
#pragma warning restore 612, 618
        }
    }
}
