using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoadSense.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bridges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoadRef = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Km = table.Column<double>(type: "float", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bridges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataSources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Period = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HourWeekdayStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Weekday = table.Column<int>(type: "int", nullable: false),
                    Hour = table.Column<int>(type: "int", nullable: false),
                    Ptp = table.Column<int>(type: "int", nullable: false),
                    Killed = table.Column<int>(type: "int", nullable: false),
                    Injured = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HourWeekdayStats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Oblasts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oblasts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tunnels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoadRef = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Km = table.Column<double>(type: "float", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tunnels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OblastMonthlyPtps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OblastId = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Ptp = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OblastMonthlyPtps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OblastMonthlyPtps_Oblasts_OblastId",
                        column: x => x.OblastId,
                        principalTable: "Oblasts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OblastPeriodStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OblastId = table.Column<int>(type: "int", nullable: false),
                    Period = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Ptp = table.Column<int>(type: "int", nullable: false),
                    Killed = table.Column<int>(type: "int", nullable: false),
                    Injured = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OblastPeriodStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OblastPeriodStats_Oblasts_OblastId",
                        column: x => x.OblastId,
                        principalTable: "Oblasts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeatherDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OblastId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    TempMin = table.Column<double>(type: "float", nullable: false),
                    TempMax = table.Column<double>(type: "float", nullable: false),
                    TempMean = table.Column<double>(type: "float", nullable: false),
                    PrecipitationMm = table.Column<double>(type: "float", nullable: false),
                    SnowfallCm = table.Column<double>(type: "float", nullable: false),
                    HumidityMean = table.Column<double>(type: "float", nullable: false),
                    CloudMean = table.Column<double>(type: "float", nullable: false),
                    WindMaxKmh = table.Column<double>(type: "float", nullable: false),
                    GustMaxKmh = table.Column<double>(type: "float", nullable: false),
                    WeatherCodeMax = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeatherDays_Oblasts_OblastId",
                        column: x => x.OblastId,
                        principalTable: "Oblasts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HourWeekdayStats_Weekday_Hour",
                table: "HourWeekdayStats",
                columns: new[] { "Weekday", "Hour" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OblastMonthlyPtps_OblastId_Year_Month",
                table: "OblastMonthlyPtps",
                columns: new[] { "OblastId", "Year", "Month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OblastPeriodStats_OblastId_Period",
                table: "OblastPeriodStats",
                columns: new[] { "OblastId", "Period" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Oblasts_Name",
                table: "Oblasts",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeatherDays_OblastId_Date",
                table: "WeatherDays",
                columns: new[] { "OblastId", "Date" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bridges");

            migrationBuilder.DropTable(
                name: "DataSources");

            migrationBuilder.DropTable(
                name: "HourWeekdayStats");

            migrationBuilder.DropTable(
                name: "OblastMonthlyPtps");

            migrationBuilder.DropTable(
                name: "OblastPeriodStats");

            migrationBuilder.DropTable(
                name: "Tunnels");

            migrationBuilder.DropTable(
                name: "WeatherDays");

            migrationBuilder.DropTable(
                name: "Oblasts");
        }
    }
}
