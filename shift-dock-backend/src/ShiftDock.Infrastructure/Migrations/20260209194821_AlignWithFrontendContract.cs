using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftDock.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlignWithFrontendContract : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkerAssignments_Projects_ProjectId",
                table: "WorkerAssignments");

            migrationBuilder.DropIndex(
                name: "IX_WorkerAssignments_ProjectId",
                table: "WorkerAssignments");

            migrationBuilder.DropIndex(
                name: "IX_WorkerAssignments_ProjectId_UserId",
                table: "WorkerAssignments");

            migrationBuilder.DropIndex(
                name: "IX_Shifts_Date",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "AssignAll",
                table: "WorkerAssignments");

            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Rate",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "WorkType",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ExcludeSaturday",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ExcludeSunday",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Rate",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ScheduleType",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "WorkType",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "WorkerAssignments",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "AssignedShifts",
                table: "WorkerAssignments",
                newName: "ShiftId");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Shifts",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "WarehouseName",
                table: "Projects",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "FixedStartDate",
                table: "Projects",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "FixedEndDate",
                table: "Projects",
                newName: "EndDate");

            migrationBuilder.RenameColumn(
                name: "RatePerHour",
                table: "Organizations",
                newName: "DefaultHourlyRate");

            migrationBuilder.RenameColumn(
                name: "RatePerContainer",
                table: "Organizations",
                newName: "DefaultContainerRate");

            migrationBuilder.RenameColumn(
                name: "RatePerBox",
                table: "Organizations",
                newName: "DefaultBoxRate");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "Organizations",
                newName: "JoinCode");

            migrationBuilder.RenameIndex(
                name: "IX_Organizations_Code",
                table: "Organizations",
                newName: "IX_Organizations_JoinCode");

            migrationBuilder.AddColumn<int>(
                name: "ActualQuantity",
                table: "WorkerAssignments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "WorkerAssignments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StartTime",
                table: "Shifts",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "interval");

            migrationBuilder.AlterColumn<string>(
                name: "EndTime",
                table: "Shifts",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "interval");

            migrationBuilder.AddColumn<string>(
                name: "ShiftDate",
                table: "Shifts",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TargetQuantity",
                table: "Shifts",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Projects",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Projects",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Projects",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerAssignments_ShiftId",
                table: "WorkerAssignments",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerAssignments_ShiftId_UserId",
                table: "WorkerAssignments",
                columns: new[] { "ShiftId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_ShiftDate",
                table: "Shifts",
                column: "ShiftDate");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerAssignments_Shifts_ShiftId",
                table: "WorkerAssignments",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkerAssignments_Shifts_ShiftId",
                table: "WorkerAssignments");

            migrationBuilder.DropIndex(
                name: "IX_WorkerAssignments_ShiftId",
                table: "WorkerAssignments");

            migrationBuilder.DropIndex(
                name: "IX_WorkerAssignments_ShiftId_UserId",
                table: "WorkerAssignments");

            migrationBuilder.DropIndex(
                name: "IX_Shifts_ShiftDate",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "ActualQuantity",
                table: "WorkerAssignments");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "WorkerAssignments");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ShiftDate",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "TargetQuantity",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "WorkerAssignments",
                newName: "ProjectId");

            migrationBuilder.RenameColumn(
                name: "ShiftId",
                table: "WorkerAssignments",
                newName: "AssignedShifts");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Shifts",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Projects",
                newName: "FixedStartDate");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Projects",
                newName: "WarehouseName");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "Projects",
                newName: "FixedEndDate");

            migrationBuilder.RenameColumn(
                name: "JoinCode",
                table: "Organizations",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "DefaultHourlyRate",
                table: "Organizations",
                newName: "RatePerHour");

            migrationBuilder.RenameColumn(
                name: "DefaultContainerRate",
                table: "Organizations",
                newName: "RatePerContainer");

            migrationBuilder.RenameColumn(
                name: "DefaultBoxRate",
                table: "Organizations",
                newName: "RatePerBox");

            migrationBuilder.RenameIndex(
                name: "IX_Organizations_JoinCode",
                table: "Organizations",
                newName: "IX_Organizations_Code");

            migrationBuilder.AddColumn<bool>(
                name: "AssignAll",
                table: "WorkerAssignments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "StartTime",
                table: "Shifts",
                type: "interval",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "EndTime",
                table: "Shifts",
                type: "interval",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AddColumn<decimal>(
                name: "Rate",
                table: "Shifts",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "WorkType",
                table: "Shifts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Projects",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "ExcludeSaturday",
                table: "Projects",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ExcludeSunday",
                table: "Projects",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Rate",
                table: "Projects",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ScheduleType",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WorkType",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerAssignments_ProjectId",
                table: "WorkerAssignments",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerAssignments_ProjectId_UserId",
                table: "WorkerAssignments",
                columns: new[] { "ProjectId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_Date",
                table: "Shifts",
                column: "Date");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerAssignments_Projects_ProjectId",
                table: "WorkerAssignments",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
