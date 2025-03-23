﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ZenlessZoneZeroWiki.Data;

#nullable disable

namespace ZenlessZoneZeroWiki.Migrations
{
    [DbContext(typeof(ZenlessZoneZeroContext))]
    [Migration("20250323145502_AddTypeUrllinkToCharacter")]
    partial class AddTypeUrllinkToCharacter
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("ZenlessZoneZeroWiki.Models.Character", b =>
                {
                    b.Property<int>("CharacterID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("CharacterID"));

                    b.Property<int>("Attack")
                        .HasColumnType("int");

                    b.Property<int>("Defence")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Element")
                        .HasColumnType("int");

                    b.Property<string>("FactionimageUrllink")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("HP")
                        .HasColumnType("int");

                    b.Property<string>("ImageUrllink")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("TypeUrllink")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("faction")
                        .HasColumnType("int");

                    b.HasKey("CharacterID");

                    b.ToTable("Characters");
                });

            modelBuilder.Entity("ZenlessZoneZeroWiki.Models.Favourite", b =>
                {
                    b.Property<string>("FirebaseUid")
                        .HasColumnType("varchar(255)");

                    b.Property<int>("CharacterID")
                        .HasColumnType("int");

                    b.Property<int>("WeaponID")
                        .HasColumnType("int");

                    b.Property<int>("FavoriteID")
                        .HasColumnType("int");

                    b.Property<DateTime>("TimeModified")
                        .HasColumnType("datetime(6)");

                    b.HasKey("FirebaseUid", "CharacterID", "WeaponID");

                    b.HasIndex("CharacterID");

                    b.HasIndex("WeaponID");

                    b.ToTable("Favourites");
                });

            modelBuilder.Entity("ZenlessZoneZeroWiki.Models.User", b =>
                {
                    b.Property<string>("FirebaseUid")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("TimeCreated")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.HasKey("FirebaseUid");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ZenlessZoneZeroWiki.Models.Weapon", b =>
                {
                    b.Property<int>("WeaponID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("WeaponID"));

                    b.Property<int>("AttackDMG")
                        .HasColumnType("int");

                    b.Property<int>("Defence")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ImageUrllink")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("WeaponID");

                    b.ToTable("Weapons");
                });

            modelBuilder.Entity("ZenlessZoneZeroWiki.Models.Favourite", b =>
                {
                    b.HasOne("ZenlessZoneZeroWiki.Models.Character", "Character")
                        .WithMany("Favourites")
                        .HasForeignKey("CharacterID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ZenlessZoneZeroWiki.Models.User", "User")
                        .WithMany("Favourites")
                        .HasForeignKey("FirebaseUid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ZenlessZoneZeroWiki.Models.Weapon", "Weapon")
                        .WithMany("Favourites")
                        .HasForeignKey("WeaponID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Character");

                    b.Navigation("User");

                    b.Navigation("Weapon");
                });

            modelBuilder.Entity("ZenlessZoneZeroWiki.Models.Character", b =>
                {
                    b.Navigation("Favourites");
                });

            modelBuilder.Entity("ZenlessZoneZeroWiki.Models.User", b =>
                {
                    b.Navigation("Favourites");
                });

            modelBuilder.Entity("ZenlessZoneZeroWiki.Models.Weapon", b =>
                {
                    b.Navigation("Favourites");
                });
#pragma warning restore 612, 618
        }
    }
}
