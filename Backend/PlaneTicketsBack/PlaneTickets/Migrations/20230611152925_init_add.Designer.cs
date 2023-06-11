﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PlaneTickets.Persistence;

#nullable disable

namespace PlaneTickets.Migrations
{
    [DbContext(typeof(PlaneTicketsDbContext))]
    [Migration("20230611152925_init_add")]
    partial class init_add
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.5");

            modelBuilder.Entity("PlaneTickets.Models.Airport", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Airports");
                });

            modelBuilder.Entity("PlaneTickets.Models.Flight", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("AirportArrivalPlaceId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("AirportDeparturePlaceId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("ArrivalDateTime")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DepartureDateTime")
                        .HasColumnType("TEXT");

                    b.Property<int>("FlightStatus")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PassengerNumber")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Transfers")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AirportArrivalPlaceId");

                    b.HasIndex("AirportDeparturePlaceId");

                    b.ToTable("Flights");
                });

            modelBuilder.Entity("PlaneTickets.Models.Reservation", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("FlightId")
                        .HasColumnType("TEXT");

                    b.Property<int>("ReservationStatus")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Tickets")
                        .HasColumnType("INTEGER");

                    b.Property<string>("_UserUsername")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("FlightId");

                    b.HasIndex("_UserUsername");

                    b.ToTable("Reservations");
                });

            modelBuilder.Entity("PlaneTickets.Models.UserDb", b =>
                {
                    b.Property<string>("Username")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Username");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PlaneTickets.Models.Flight", b =>
                {
                    b.HasOne("PlaneTickets.Models.Airport", "AirportArrivalPlace")
                        .WithMany()
                        .HasForeignKey("AirportArrivalPlaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PlaneTickets.Models.Airport", "AirportDeparturePlace")
                        .WithMany()
                        .HasForeignKey("AirportDeparturePlaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AirportArrivalPlace");

                    b.Navigation("AirportDeparturePlace");
                });

            modelBuilder.Entity("PlaneTickets.Models.Reservation", b =>
                {
                    b.HasOne("PlaneTickets.Models.Flight", "_Flight")
                        .WithMany()
                        .HasForeignKey("FlightId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PlaneTickets.Models.UserDb", "_User")
                        .WithMany()
                        .HasForeignKey("_UserUsername")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("_Flight");

                    b.Navigation("_User");
                });
#pragma warning restore 612, 618
        }
    }
}
