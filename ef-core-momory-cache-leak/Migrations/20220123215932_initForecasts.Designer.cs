﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ef_core_momory_cache_leak;

#nullable disable

namespace ef_core_momory_cache_leak.Migrations
{
    [DbContext(typeof(TodoContext))]
    [Migration("20220123215932_initForecasts")]
    partial class initForecasts
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("ef_core_momory_cache_leak.WeatherForecast", b =>
                {
                    b.Property<int>("ForcastNumber")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Summary")
                        .HasColumnType("longtext");

                    b.Property<int>("TemperatureC")
                        .HasColumnType("int");

                    b.HasKey("ForcastNumber");

                    b.ToTable("Forecasts", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
