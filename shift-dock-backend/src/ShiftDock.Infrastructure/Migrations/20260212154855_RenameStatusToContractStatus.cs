using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftDock.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameStatusToContractStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Projects",
                newName: "ContractStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContractStatus",
                table: "Projects",
                newName: "Status");
        }
    }
}
