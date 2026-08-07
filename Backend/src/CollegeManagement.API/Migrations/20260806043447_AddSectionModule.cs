using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollegeManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSectionModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sections",
                columns: table => new
                {
                    SectionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Board = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    Group = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AcademicLevel = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SectionName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RoomNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClassTeacherId = table.Column<int>(type: "int", nullable: true),
                    MaximumStrength = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sections", x => x.SectionId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // --- Stored Procedures UP ---

            // 1. sp_GetAllSections
            migrationBuilder.Sql(@"
DROP PROCEDURE IF EXISTS sp_GetAllSections;
", suppressTransaction: true);
            migrationBuilder.Sql(@"
CREATE PROCEDURE sp_GetAllSections()
BEGIN
    SELECT s.SectionId, s.Board, s.AcademicYearId, ay.AcademicYearName, s.Group, s.AcademicLevel,
           s.SectionName, s.RoomNumber, s.ClassTeacherId, 
           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS ClassTeacherName,
           s.MaximumStrength, s.IsActive, s.CreatedAt, s.UpdatedAt
    FROM Sections s
    LEFT JOIN AcademicYears ay ON ay.AcademicYearId = s.AcademicYearId
    LEFT JOIN Faculties f ON f.Id = s.ClassTeacherId
    ORDER BY s.SectionId DESC;
END;
", suppressTransaction: true);

            // 2. sp_GetSectionById
            migrationBuilder.Sql(@"
DROP PROCEDURE IF EXISTS sp_GetSectionById;
", suppressTransaction: true);
            migrationBuilder.Sql(@"
CREATE PROCEDURE sp_GetSectionById(IN p_SectionId INT)
BEGIN
    SELECT s.SectionId, s.Board, s.AcademicYearId, ay.AcademicYearName, s.Group, s.AcademicLevel,
           s.SectionName, s.RoomNumber, s.ClassTeacherId, 
           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS ClassTeacherName,
           s.MaximumStrength, s.IsActive, s.CreatedAt, s.UpdatedAt
    FROM Sections s
    LEFT JOIN AcademicYears ay ON ay.AcademicYearId = s.AcademicYearId
    LEFT JOIN Faculties f ON f.Id = s.ClassTeacherId
    WHERE s.SectionId = p_SectionId;
END;
", suppressTransaction: true);

            // 3. sp_CreateSection
            migrationBuilder.Sql(@"
DROP PROCEDURE IF EXISTS sp_CreateSection;
", suppressTransaction: true);
            migrationBuilder.Sql(@"
CREATE PROCEDURE sp_CreateSection(
    IN p_Board VARCHAR(100),
    IN p_AcademicYearId INT,
    IN p_Group VARCHAR(100),
    IN p_AcademicLevel VARCHAR(50),
    IN p_SectionName VARCHAR(50),
    IN p_RoomNumber VARCHAR(50),
    IN p_ClassTeacherId INT,
    IN p_MaximumStrength INT,
    IN p_IsActive TINYINT(1)
)
BEGIN
    INSERT INTO Sections (Board, AcademicYearId, `Group`, AcademicLevel, SectionName, RoomNumber, ClassTeacherId, MaximumStrength, IsActive, CreatedAt)
    VALUES (p_Board, p_AcademicYearId, p_Group, p_AcademicLevel, p_SectionName, p_RoomNumber, p_ClassTeacherId, p_MaximumStrength, p_IsActive, UTC_TIMESTAMP());
    
    SELECT LAST_INSERT_ID();
END;
", suppressTransaction: true);

            // 4. sp_UpdateSection
            migrationBuilder.Sql(@"
DROP PROCEDURE IF EXISTS sp_UpdateSection;
", suppressTransaction: true);
            migrationBuilder.Sql(@"
CREATE PROCEDURE sp_UpdateSection(
    IN p_SectionId INT,
    IN p_Board VARCHAR(100),
    IN p_AcademicYearId INT,
    IN p_Group VARCHAR(100),
    IN p_AcademicLevel VARCHAR(50),
    IN p_SectionName VARCHAR(50),
    IN p_RoomNumber VARCHAR(50),
    IN p_ClassTeacherId INT,
    IN p_MaximumStrength INT,
    IN p_IsActive TINYINT(1)
)
BEGIN
    UPDATE Sections
    SET Board = p_Board,
        AcademicYearId = p_AcademicYearId,
        `Group` = p_Group,
        AcademicLevel = p_AcademicLevel,
        SectionName = p_SectionName,
        RoomNumber = p_RoomNumber,
        ClassTeacherId = p_ClassTeacherId,
        MaximumStrength = p_MaximumStrength,
        IsActive = p_IsActive,
        UpdatedAt = UTC_TIMESTAMP()
    WHERE SectionId = p_SectionId;
END;
", suppressTransaction: true);

            // 5. sp_DeleteSection
            migrationBuilder.Sql(@"
DROP PROCEDURE IF EXISTS sp_DeleteSection;
", suppressTransaction: true);
            migrationBuilder.Sql(@"
CREATE PROCEDURE sp_DeleteSection(IN p_SectionId INT)
BEGIN
    DELETE FROM Sections WHERE SectionId = p_SectionId;
END;
", suppressTransaction: true);

            // 6. sp_GetSectionsByGroup
            migrationBuilder.Sql(@"
DROP PROCEDURE IF EXISTS sp_GetSectionsByGroup;
", suppressTransaction: true);
            migrationBuilder.Sql(@"
CREATE PROCEDURE sp_GetSectionsByGroup(IN p_GroupId INT)
BEGIN
    SELECT s.SectionId, s.Board, s.AcademicYearId, ay.AcademicYearName, s.Group, s.AcademicLevel,
           s.SectionName, s.RoomNumber, s.ClassTeacherId, 
           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS ClassTeacherName,
           s.MaximumStrength, s.IsActive, s.CreatedAt, s.UpdatedAt
    FROM Sections s
    INNER JOIN `Groups` g ON g.Board = s.Board 
                         AND g.AcademicYearId = s.AcademicYearId 
                         AND g.GroupName = s.Group 
                         AND g.AcademicLevel = s.AcademicLevel
    LEFT JOIN AcademicYears ay ON ay.AcademicYearId = s.AcademicYearId
    LEFT JOIN Faculties f ON f.Id = s.ClassTeacherId
    WHERE g.GroupId = p_GroupId
    ORDER BY s.SectionId DESC;
END;
", suppressTransaction: true);

            // 7. sp_ValidateSectionName
            migrationBuilder.Sql(@"
DROP PROCEDURE IF EXISTS sp_ValidateSectionName;
", suppressTransaction: true);
            migrationBuilder.Sql(@"
CREATE PROCEDURE sp_ValidateSectionName(
    IN p_Board VARCHAR(100),
    IN p_AcademicYearId INT,
    IN p_Group VARCHAR(100),
    IN p_AcademicLevel VARCHAR(50),
    IN p_SectionName VARCHAR(50),
    IN p_ExcludeSectionId INT
)
BEGIN
    SELECT COUNT(1)
    FROM Sections
    WHERE Board = p_Board
      AND AcademicYearId = p_AcademicYearId
      AND `Group` = p_Group
      AND AcademicLevel = p_AcademicLevel
      AND BINARY SectionName = BINARY p_SectionName
      AND (p_ExcludeSectionId IS NULL OR SectionId <> p_ExcludeSectionId);
END;
", suppressTransaction: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetAllSections;", suppressTransaction: true);
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetSectionById;", suppressTransaction: true);
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_CreateSection;", suppressTransaction: true);
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_UpdateSection;", suppressTransaction: true);
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_DeleteSection;", suppressTransaction: true);
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetSectionsByGroup;", suppressTransaction: true);
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ValidateSectionName;", suppressTransaction: true);

            migrationBuilder.DropTable(
                name: "Sections");
        }
    }
}
