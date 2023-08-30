﻿// <auto-generated />
using System;
using Masterdev.Update.RestAPI.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Masterdev.Update.RestAPI.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230802121033_one")]
    partial class one
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Masterdev.Update.RestAPI.Entity.Data", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("createdby")
                        .HasColumnType("int");

                    b.Property<DateTime>("date")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("file_id")
                        .HasColumnType("int");

                    b.Property<bool>("published")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("version")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.ToTable("data");
                });

            modelBuilder.Entity("Masterdev.Update.RestAPI.Entity.Files", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("createdby")
                        .HasColumnType("int");

                    b.Property<DateTime>("date")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("file_name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("file_path")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.ToTable("files");
                });

            modelBuilder.Entity("Masterdev.Update.RestAPI.Entity.Users", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.ToTable("users");
                });
#pragma warning restore 612, 618
        }
    }
}