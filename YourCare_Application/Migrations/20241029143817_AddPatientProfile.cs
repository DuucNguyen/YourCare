using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourCare_Application.Migrations
{
    public partial class AddPatientProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointment_Patient",
                table: "Appointment");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointment_Status",
                table: "Appointment");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropIndex(
                name: "IX_Appointment_StatusID",
                table: "Appointment");

            migrationBuilder.DropColumn(
                name: "InsuranceNumber",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "StatusID",
                table: "Appointment",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "PatientID",
                table: "Appointment",
                newName: "PatientProfileID");

            migrationBuilder.RenameIndex(
                name: "IX_Appointment_PatientID",
                table: "Appointment",
                newName: "IX_Appointment_PatientProfileID");

            migrationBuilder.CreateTable(
                name: "PatientProfile",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ApplicationUserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Gender = table.Column<bool>(type: "bit", nullable: true),
                    Dob = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IdentityNumber = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    InsuranceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    Career = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Ethnic = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientProfile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Patient_User",
                        column: x => x.ApplicationUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientProfile_ApplicationUserID",
                table: "PatientProfile",
                column: "ApplicationUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Patient_Appointment",
                table: "Appointment",
                column: "PatientProfileID",
                principalTable: "PatientProfile",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patient_Appointment",
                table: "Appointment");

            migrationBuilder.DropTable(
                name: "PatientProfile");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Appointment",
                newName: "StatusID");

            migrationBuilder.RenameColumn(
                name: "PatientProfileID",
                table: "Appointment",
                newName: "PatientID");

            migrationBuilder.RenameIndex(
                name: "IX_Appointment_PatientProfileID",
                table: "Appointment",
                newName: "IX_Appointment_PatientID");

            migrationBuilder.AddColumn<string>(
                name: "InsuranceNumber",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_StatusID",
                table: "Appointment",
                column: "StatusID");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointment_Patient",
                table: "Appointment",
                column: "PatientID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointment_Status",
                table: "Appointment",
                column: "StatusID",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
