﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PhoneAssistant.WPF.Application;

#nullable disable

namespace PhoneAssistant.WPF.Migrations
{
    [DbContext(typeof(PhoneAssistantDbContext))]
    [Migration("20240410121903_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.4");

            modelBuilder.Entity("PhoneAssistant.WPF.Application.Entities.Disposal", b =>
                {
                    b.Property<string>("Imei")
                        .HasColumnType("TEXT");

                    b.Property<string>("Action")
                        .HasColumnType("TEXT");

                    b.Property<int?>("Certificate")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("SR")
                        .HasColumnType("INTEGER");

                    b.Property<string>("StatusMS")
                        .HasColumnType("TEXT");

                    b.Property<string>("StatusPA")
                        .HasColumnType("TEXT");

                    b.Property<string>("StatusSCC")
                        .HasColumnType("TEXT");

                    b.HasKey("Imei");

                    b.ToTable("ReconcileDisposals", (string)null);
                });

            modelBuilder.Entity("PhoneAssistant.WPF.Application.Entities.EEBaseReport", b =>
                {
                    b.Property<string>("PhoneNumber")
                        .HasColumnType("TEXT");

                    b.Property<string>("ConnectedIMEI")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ContractEndDate")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Handset")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("LastUsedIMEI")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SIMNumber")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TalkPlan")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("PhoneNumber");

                    b.ToTable("BaseReport", (string)null);
                });

            modelBuilder.Entity("PhoneAssistant.WPF.Application.Entities.ImportHistory", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<string>("File")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ImportDate")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ImportHistory", (string)null);
                });

            modelBuilder.Entity("PhoneAssistant.WPF.Application.Entities.Location", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("PrintDate")
                        .HasColumnType("INTEGER");

                    b.HasKey("Name");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("PhoneAssistant.WPF.Application.Entities.Phone", b =>
                {
                    b.Property<string>("Imei")
                        .HasColumnType("TEXT")
                        .HasColumnName("IMEI");

                    b.Property<string>("AssetTag")
                        .HasColumnType("TEXT");

                    b.Property<int?>("Collection")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Condition")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("NorR");

                    b.Property<string>("DespatchDetails")
                        .HasColumnType("TEXT");

                    b.Property<string>("FormerUser")
                        .HasColumnType("TEXT");

                    b.Property<string>("LastUpdate")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("NewUser")
                        .HasColumnType("TEXT");

                    b.Property<string>("Notes")
                        .HasColumnType("TEXT");

                    b.Property<string>("OEM")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("TEXT");

                    b.Property<int?>("SR")
                        .HasColumnType("INTEGER")
                        .HasColumnName("SRNumber");

                    b.Property<string>("SimNumber")
                        .HasColumnType("TEXT");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Imei");

                    b.HasIndex(new[] { "AssetTag" }, "IX_Phones_AssetTag")
                        .IsUnique();

                    b.ToTable("Phones", t =>
                        {
                            t.HasCheckConstraint("CK_NorR", "\"NorR\" = 'N' OR \"NorR\" = 'R'");

                            t.HasCheckConstraint("CK_OEM", "\"OEM\" = 'Apple' OR \"OEM\" = 'Nokia' OR \"OEM\" = 'Samsung' OR \"OEM\" = 'Other'");
                        });
                });

            modelBuilder.Entity("PhoneAssistant.WPF.Application.Entities.Sim", b =>
                {
                    b.Property<string>("PhoneNumber")
                        .HasColumnType("TEXT");

                    b.Property<string>("AssetTag")
                        .HasColumnType("TEXT");

                    b.Property<string>("LastUpdate")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("Notes")
                        .HasColumnType("TEXT");

                    b.Property<int?>("SR")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SimNumber")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Status")
                        .HasColumnType("TEXT");

                    b.HasKey("PhoneNumber");

                    b.HasIndex(new[] { "SimNumber" }, "IX_Sims_SimNumber")
                        .IsUnique();

                    b.ToTable("SIMs", (string)null);
                });

            modelBuilder.Entity("PhoneAssistant.WPF.Application.Entities.UpdateHistoryPhone", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AssetTag")
                        .HasColumnType("TEXT");

                    b.Property<string>("Condition")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("NorR");

                    b.Property<string>("DespatchDetails")
                        .HasColumnType("TEXT");

                    b.Property<string>("FormerUser")
                        .HasColumnType("TEXT");

                    b.Property<string>("Imei")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("IMEI");

                    b.Property<string>("LastUpdate")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("NewUser")
                        .HasColumnType("TEXT");

                    b.Property<string>("Notes")
                        .HasColumnType("TEXT");

                    b.Property<string>("OEM")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("TEXT");

                    b.Property<int?>("SR")
                        .HasColumnType("INTEGER")
                        .HasColumnName("SRNumber");

                    b.Property<string>("SimNumber")
                        .HasColumnType("INTEGER")
                        .HasColumnName("SIMNumber");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdateType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("UpdateHistoryPhones");
                });
#pragma warning restore 612, 618
        }
    }
}
