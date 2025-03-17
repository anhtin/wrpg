﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Wrpg;

#nullable disable

namespace Wrpg.Infrastructure.Database.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250317223842_AddMaxLengthOnAdventureLocationName")]
    partial class AddMaxLengthOnAdventureLocationName
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Wrpg.Adventure", b =>
                {
                    b.Property<int>("InternalId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("InternalId"));

                    b.Property<Guid>("CharacterId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("LocationName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("InternalId");

                    b.HasIndex("CharacterId");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("Adventures");
                });

            modelBuilder.Entity("Wrpg.Character", b =>
                {
                    b.Property<int>("InternalId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("InternalId"));

                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("InternalId");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("Name");

                    b.HasIndex("UserId");

                    b.ToTable("Characters");
                });

            modelBuilder.Entity("Wrpg.Character", b =>
                {
                    b.OwnsOne("Wrpg.Stats", "Stats", b1 =>
                        {
                            b1.Property<int>("CharacterInternalId")
                                .HasColumnType("integer");

                            b1.HasKey("CharacterInternalId");

                            b1.ToTable("Characters");

                            b1.WithOwner()
                                .HasForeignKey("CharacterInternalId");

                            b1.OwnsOne("Wrpg.Attributes", "Attributes", b2 =>
                                {
                                    b2.Property<int>("StatsCharacterInternalId")
                                        .HasColumnType("integer");

                                    b2.Property<int>("Constitution")
                                        .HasColumnType("integer");

                                    b2.Property<int>("Dexterity")
                                        .HasColumnType("integer");

                                    b2.Property<int>("Intelligence")
                                        .HasColumnType("integer");

                                    b2.Property<int>("Level")
                                        .HasColumnType("integer");

                                    b2.Property<int>("Spirit")
                                        .HasColumnType("integer");

                                    b2.Property<int>("Strength")
                                        .HasColumnType("integer");

                                    b2.HasKey("StatsCharacterInternalId");

                                    b2.ToTable("Characters");

                                    b2.WithOwner()
                                        .HasForeignKey("StatsCharacterInternalId");
                                });

                            b1.OwnsOne("Wrpg.Resources", "Resources", b2 =>
                                {
                                    b2.Property<int>("StatsCharacterInternalId")
                                        .HasColumnType("integer");

                                    b2.Property<int>("Energy")
                                        .HasColumnType("integer");

                                    b2.Property<int>("Health")
                                        .HasColumnType("integer");

                                    b2.HasKey("StatsCharacterInternalId");

                                    b2.ToTable("Characters");

                                    b2.WithOwner()
                                        .HasForeignKey("StatsCharacterInternalId");
                                });

                            b1.Navigation("Attributes")
                                .IsRequired();

                            b1.Navigation("Resources")
                                .IsRequired();
                        });

                    b.Navigation("Stats")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
