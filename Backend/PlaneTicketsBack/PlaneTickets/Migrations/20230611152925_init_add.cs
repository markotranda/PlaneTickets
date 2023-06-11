using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlaneTickets.Migrations
{
    /// <inheritdoc />
    public partial class init_add : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Airports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Airports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "BLOB", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "BLOB", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Username);
                });

            migrationBuilder.CreateTable(
                name: "Flights",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AirportDeparturePlaceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AirportArrivalPlaceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DepartureDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ArrivalDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Transfers = table.Column<int>(type: "INTEGER", nullable: false),
                    PassengerNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    FlightStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flights", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Flights_Airports_AirportArrivalPlaceId",
                        column: x => x.AirportArrivalPlaceId,
                        principalTable: "Airports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Flights_Airports_AirportDeparturePlaceId",
                        column: x => x.AirportDeparturePlaceId,
                        principalTable: "Airports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FlightId = table.Column<Guid>(type: "TEXT", nullable: false),
                    _UserUsername = table.Column<string>(type: "TEXT", nullable: false),
                    Tickets = table.Column<int>(type: "INTEGER", nullable: false),
                    ReservationStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_Flights_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservations_Users__UserUsername",
                        column: x => x._UserUsername,
                        principalTable: "Users",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Flights_AirportArrivalPlaceId",
                table: "Flights",
                column: "AirportArrivalPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_AirportDeparturePlaceId",
                table: "Flights",
                column: "AirportDeparturePlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations__UserUsername",
                table: "Reservations",
                column: "_UserUsername");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_FlightId",
                table: "Reservations",
                column: "FlightId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Flights");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Airports");
        }
    }
}
