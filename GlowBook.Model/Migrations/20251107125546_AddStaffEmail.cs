using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GlowBook.Model.Migrations
{
    /// <inheritdoc />
    public partial class AddStaffEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppointmentServices_AppointmentId",
                table: "AppointmentServices");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Staff",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 1,
                column: "Email",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentServices_AppointmentId_ServiceId",
                table: "AppointmentServices",
                columns: new[] { "AppointmentId", "ServiceId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppointmentServices_AppointmentId_ServiceId",
                table: "AppointmentServices");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Staff");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentServices_AppointmentId",
                table: "AppointmentServices",
                column: "AppointmentId");
        }
    }
}
