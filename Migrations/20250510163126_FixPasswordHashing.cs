using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AgriEnergyConnect.Migrations
{
    /// <inheritdoc />
    public partial class FixPasswordHashing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "MessageId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "MessageId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Farmers",
                keyColumn: "FarmerId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "CreatedDate", "Email", "FirstName", "IsActive", "LastLoginDate", "LastName", "PasswordHash", "PhoneNumber", "Role", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@agrienergyconnect.com", "Emily", true, null, "Mathews", "password", "0841258975", 1, "admin" },
                    { 2, new DateTime(2024, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "john@greenacres.com", "John", true, null, "Smith", "password", "0601091293", 0, "jsmith" }
                });

            migrationBuilder.InsertData(
                table: "Farmers",
                columns: new[] { "FarmerId", "FarmName", "Location", "UserId" },
                values: new object[] { 1, "Green Acres Farm", "Western Cape", 2 });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "MessageId", "Content", "IsRead", "RecipientId", "SenderId", "SentDate", "Subject" },
                values: new object[,]
                {
                    { 1, "Welcome to the platform. We're excited to have you join our community of farmers and green energy experts.", true, 2, 1, new DateTime(2024, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Welcome to Agri-Energy Connect" },
                    { 2, "We wanted to inform you of new government subsidies for green energy solutions on farms. Contact us for more details.", false, 2, 1, new DateTime(2024, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Important: New Green Energy Subsidies Available" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "Category", "CreatedDate", "Description", "FarmerId", "Name", "ProductionDate" },
                values: new object[,]
                {
                    { 1, "Grains", new DateTime(2024, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Non-GMO certified", 1, "Organic Maize", new DateTime(2024, 4, 26, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "Dairy", new DateTime(2024, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Large, brown eggs", 1, "Free-range Eggs", new DateTime(2024, 4, 23, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "Dairy", new DateTime(2024, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Unpasteurised", 1, "Fresh Milk", new DateTime(2024, 4, 24, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "Vegetables", new DateTime(2024, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Organic, freshly harvested", 1, "Carrots", new DateTime(2024, 4, 16, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }
    }
}
