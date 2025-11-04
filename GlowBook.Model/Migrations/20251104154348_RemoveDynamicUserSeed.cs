using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GlowBook.Model.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDynamicUserSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "role-admin", "user-admin" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "role-employee", "user-employee" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-admin");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-employee");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "DisplayName", "Email", "EmailConfirmed", "IsActive", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "user-admin", 0, "85229b69-af76-4db9-a4e3-930d70499e95", "Beheerder", "admin@glowbook.local", true, true, false, null, "ADMIN@GLOWBOOK.LOCAL", "ADMIN@GLOWBOOK.LOCAL", "AQAAAAIAAYagAAAAELWUunBcwhzVfkq6YE/gpjlaF8a33/crXJ5IZBC8cRDRlCHzYdoZX1m8k4jB17T+2w==", null, false, "c4fb060c-853b-41da-8c57-6326ed97e948", false, "admin@glowbook.local" },
                    { "user-employee", 0, "5ae8e7c4-6145-48fe-902b-325b1a85f1ff", "Medewerker", "employee@glowbook.local", true, true, false, null, "EMPLOYEE@GLOWBOOK.LOCAL", "EMPLOYEE@GLOWBOOK.LOCAL", "AQAAAAIAAYagAAAAEKJEzTDCkC4T1l6ZwpoktC6wNv3NmQ0EBkYi4IZ8b4BqwGMWJony4DS4iEn6h3fIuw==", null, false, "8b3ae2ce-8024-4c04-a2f3-8d74a3363165", false, "employee@glowbook.local" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "role-admin", "user-admin" },
                    { "role-employee", "user-employee" }
                });
        }
    }
}
