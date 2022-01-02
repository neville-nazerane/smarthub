﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SmartHub.Logic.Data;

#nullable disable

namespace SmartHub.DbMigrator.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.1");

            modelBuilder.Entity("SmartHub.Models.Entities.DeviceAction", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Capability")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Command")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Component")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("DeviceId")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("DeviceActions");
                });

            modelBuilder.Entity("SmartHub.Models.Entities.EventLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("EventId")
                        .IsRequired()
                        .HasMaxLength(90)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("EventId", "TimeStamp");

                    b.ToTable("EventLogs");
                });

            modelBuilder.Entity("SmartHub.Models.Entities.SceneAction", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("SceneId")
                        .IsRequired()
                        .HasMaxLength(90)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("SceneActions");
                });
#pragma warning restore 612, 618
        }
    }
}