using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PassIn.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckinPropertyToAttendee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CheckIns_AttendeeId",
                table: "CheckIns");

            migrationBuilder.CreateIndex(
                name: "IX_CheckIns_AttendeeId",
                table: "CheckIns",
                column: "AttendeeId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CheckIns_AttendeeId",
                table: "CheckIns");

            migrationBuilder.CreateIndex(
                name: "IX_CheckIns_AttendeeId",
                table: "CheckIns",
                column: "AttendeeId");
        }
    }
}
