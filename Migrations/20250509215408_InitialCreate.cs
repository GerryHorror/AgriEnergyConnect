using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AgriEnergyConnect.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Farmers",
                columns: table => new
                {
                    FarmerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FarmName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Farmers", x => x.FarmerId);
                    table.ForeignKey(
                        name: "FK_Farmers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenderId = table.Column<int>(type: "int", nullable: false),
                    RecipientId = table.Column<int>(type: "int", nullable: false),
                    SentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_Messages_Users_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProductionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FarmerId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Products_Farmers_FarmerId",
                        column: x => x.FarmerId,
                        principalTable: "Farmers",
                        principalColumn: "FarmerId",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Farmers_UserId",
                table: "Farmers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RecipientId",
                table: "Messages",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_FarmerId",
                table: "Products",
                column: "FarmerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Farmers");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
