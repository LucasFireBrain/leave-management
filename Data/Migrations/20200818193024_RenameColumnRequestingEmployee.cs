using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace leave_management.Data.Migrations
{
    public partial class RenameColumnRequestingEmployee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RequestingEmplyeeId",
                table: "LeaveRequests",
                newName: "RequestingEmployeeId");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
