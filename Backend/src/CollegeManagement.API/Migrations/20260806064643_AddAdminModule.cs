using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollegeManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "admins",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admins", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_admins_Email",
                table: "admins",
                column: "Email",
                unique: true);

            // --- Stored Procedures UP ---

            // 1. sp_GetAllAdmins
            migrationBuilder.Sql(@"
DROP PROCEDURE IF EXISTS sp_GetAllAdmins;
", suppressTransaction: true);
            migrationBuilder.Sql(@"
CREATE PROCEDURE sp_GetAllAdmins()
BEGIN
    SELECT id, Email, IsActive FROM admins ORDER BY id DESC;
END;
", suppressTransaction: true);

            // 2. sp_GetAdminById
            migrationBuilder.Sql(@"
DROP PROCEDURE IF EXISTS sp_GetAdminById;
", suppressTransaction: true);
            migrationBuilder.Sql(@"
CREATE PROCEDURE sp_GetAdminById(IN p_Id INT)
BEGIN
    SELECT id, Email, IsActive FROM admins WHERE id = p_Id;
END;
", suppressTransaction: true);

            // 3. sp_GetAdminByEmail
            migrationBuilder.Sql(@"
DROP PROCEDURE IF EXISTS sp_GetAdminByEmail;
", suppressTransaction: true);
            migrationBuilder.Sql(@"
CREATE PROCEDURE sp_GetAdminByEmail(IN p_Email VARCHAR(255))
BEGIN
    SELECT id, Email, Password, IsActive FROM admins WHERE Email = p_Email;
END;
", suppressTransaction: true);

            // 4. sp_CreateAdmin
            migrationBuilder.Sql(@"
DROP PROCEDURE IF EXISTS sp_CreateAdmin;
", suppressTransaction: true);
            migrationBuilder.Sql(@"
CREATE PROCEDURE sp_CreateAdmin(IN p_Email VARCHAR(255), IN p_Password VARCHAR(255), IN p_IsActive TINYINT(1))
BEGIN
    INSERT INTO admins (Email, Password, IsActive) VALUES (p_Email, p_Password, p_IsActive);
    SELECT LAST_INSERT_ID();
END;
", suppressTransaction: true);

            // 5. sp_UpdateAdminStatus
            migrationBuilder.Sql(@"
DROP PROCEDURE IF EXISTS sp_UpdateAdminStatus;
", suppressTransaction: true);
            migrationBuilder.Sql(@"
CREATE PROCEDURE sp_UpdateAdminStatus(IN p_Id INT, IN p_IsActive TINYINT(1))
BEGIN
    UPDATE admins SET IsActive = p_IsActive WHERE id = p_Id;
END;
", suppressTransaction: true);

            // 6. sp_ChangeAdminPassword
            migrationBuilder.Sql(@"
DROP PROCEDURE IF EXISTS sp_ChangeAdminPassword;
", suppressTransaction: true);
            migrationBuilder.Sql(@"
CREATE PROCEDURE sp_ChangeAdminPassword(IN p_Id INT, IN p_Password VARCHAR(255))
BEGIN
    UPDATE admins SET Password = p_Password WHERE id = p_Id;
END;
", suppressTransaction: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetAllAdmins;", suppressTransaction: true);
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetAdminById;", suppressTransaction: true);
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetAdminByEmail;", suppressTransaction: true);
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_CreateAdmin;", suppressTransaction: true);
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_UpdateAdminStatus;", suppressTransaction: true);
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ChangeAdminPassword;", suppressTransaction: true);

            migrationBuilder.DropTable(
                name: "admins");
        }
    }
}
