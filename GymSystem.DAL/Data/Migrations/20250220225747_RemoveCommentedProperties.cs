using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymSystem.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCommentedProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WorkoutPlanId",
                table: "WorkoutPlans",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "NotificationId",
                table: "Notifications",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "FeedbackId",
                table: "Feedbacks",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "BMIRecordId",
                table: "BMIRecords",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "WorkoutPlans",
                newName: "WorkoutPlanId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Notifications",
                newName: "NotificationId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Feedbacks",
                newName: "FeedbackId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "BMIRecords",
                newName: "BMIRecordId");
        }
    }
}
