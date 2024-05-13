using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhoneAssistant.WPF.Migrations
{
    /// <inheritdoc />
    public partial class Disposal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReconcileDisposals",
                columns: table => new
                {
                    Imei = table.Column<string>(type: "TEXT", nullable: false),
                    StatusMS = table.Column<string>(type: "TEXT", nullable: true),
                    StatusPA = table.Column<string>(type: "TEXT", nullable: true),
                    StatusSCC = table.Column<string>(type: "TEXT", nullable: true),
                    SR = table.Column<int>(type: "INTEGER", nullable: true),
                    Certificate = table.Column<int>(type: "INTEGER", nullable: true),
                    Action = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReconcileDisposals", x => x.Imei);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReconcileDisposals");
        }
    }
}
