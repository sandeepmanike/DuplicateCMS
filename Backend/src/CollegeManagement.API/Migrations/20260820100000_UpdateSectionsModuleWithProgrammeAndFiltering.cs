using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollegeManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSectionsModuleWithProgrammeAndFiltering : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
-- 1. Ensure Columns exist in Sections Table
SET @exist := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Sections' AND COLUMN_NAME = 'Programme');
SET @sqlstmt := IF(@exist = 0, 'ALTER TABLE `Sections` ADD COLUMN `Programme` VARCHAR(100) NOT NULL DEFAULT \'\' AFTER `Group`', 'SELECT 1');
PREPARE stmt FROM @sqlstmt; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @exist := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Sections' AND COLUMN_NAME = 'RoomId');
SET @sqlstmt := IF(@exist = 0, 'ALTER TABLE `Sections` ADD COLUMN `RoomId` INT NULL AFTER `RoomNumber`', 'SELECT 1');
PREPARE stmt FROM @sqlstmt; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @exist := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Sections' AND COLUMN_NAME = 'BoardId');
SET @sqlstmt := IF(@exist = 0, 'ALTER TABLE `Sections` ADD COLUMN `BoardId` INT NULL AFTER `SectionId`', 'SELECT 1');
PREPARE stmt FROM @sqlstmt; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @exist := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Sections' AND COLUMN_NAME = 'GroupId');
SET @sqlstmt := IF(@exist = 0, 'ALTER TABLE `Sections` ADD COLUMN `GroupId` INT NULL AFTER `AcademicYearId`', 'SELECT 1');
PREPARE stmt FROM @sqlstmt; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- 2. sp_GetAllSections (With Search and Filters)
DROP PROCEDURE IF EXISTS sp_GetAllSections;
CREATE PROCEDURE sp_GetAllSections(
    IN p_Board VARCHAR(100),
    IN p_AcademicYearId INT,
    IN p_Group VARCHAR(100),
    IN p_GroupId INT,
    IN p_Programme VARCHAR(100),
    IN p_AcademicLevel VARCHAR(50),
    IN p_SearchTerm VARCHAR(100),
    IN p_IsActive TINYINT(1)
)
BEGIN
    SELECT s.SectionId,
           s.BoardId,
           s.Board,
           s.AcademicYearId,
           COALESCE(ay.AcademicYearName, '') AS AcademicYearName,
           s.GroupId,
           s.`Group`,
           COALESCE(s.Programme, '') AS Programme,
           s.AcademicLevel,
           s.SectionName,
           s.RoomNumber,
           s.RoomId,
           COALESCE(r.RoomName, s.RoomNumber, '') AS RoomName,
           s.ClassTeacherId,
           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS ClassTeacherName,
           s.MaximumStrength,
           s.IsActive,
           s.CreatedAt,
           s.UpdatedAt
    FROM Sections s
    LEFT JOIN AcademicYears ay ON ay.AcademicYearId = s.AcademicYearId
    LEFT JOIN Faculties f ON f.Id = s.ClassTeacherId
    LEFT JOIN Rooms r ON r.RoomId = s.RoomId
    WHERE (p_Board IS NULL OR p_Board = '' OR s.Board = p_Board)
      AND (p_AcademicYearId IS NULL OR p_AcademicYearId = 0 OR s.AcademicYearId = p_AcademicYearId)
      AND (p_Group IS NULL OR p_Group = '' OR s.`Group` = p_Group)
      AND (p_GroupId IS NULL OR p_GroupId = 0 OR s.GroupId = p_GroupId)
      AND (p_Programme IS NULL OR p_Programme = '' OR s.Programme = p_Programme)
      AND (p_AcademicLevel IS NULL OR p_AcademicLevel = '' OR s.AcademicLevel = p_AcademicLevel)
      AND (p_IsActive IS NULL OR s.IsActive = p_IsActive)
      AND (p_SearchTerm IS NULL OR p_SearchTerm = '' OR (
           s.SectionName LIKE CONCAT('%', p_SearchTerm, '%') OR
           s.`Group` LIKE CONCAT('%', p_SearchTerm, '%') OR
           s.Programme LIKE CONCAT('%', p_SearchTerm, '%') OR
           CONCAT(f.FirstName, ' ', f.LastName) LIKE CONCAT('%', p_SearchTerm, '%') OR
           s.RoomNumber LIKE CONCAT('%', p_SearchTerm, '%') OR
           r.RoomName LIKE CONCAT('%', p_SearchTerm, '%')
      ))
    ORDER BY s.SectionId DESC;
END;

-- 3. sp_GetSectionById
DROP PROCEDURE IF EXISTS sp_GetSectionById;
CREATE PROCEDURE sp_GetSectionById(IN p_SectionId INT)
BEGIN
    SELECT s.SectionId,
           s.BoardId,
           s.Board,
           s.AcademicYearId,
           COALESCE(ay.AcademicYearName, '') AS AcademicYearName,
           s.GroupId,
           s.`Group`,
           COALESCE(s.Programme, '') AS Programme,
           s.AcademicLevel,
           s.SectionName,
           s.RoomNumber,
           s.RoomId,
           COALESCE(r.RoomName, s.RoomNumber, '') AS RoomName,
           s.ClassTeacherId,
           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS ClassTeacherName,
           s.MaximumStrength,
           s.IsActive,
           s.CreatedAt,
           s.UpdatedAt
    FROM Sections s
    LEFT JOIN AcademicYears ay ON ay.AcademicYearId = s.AcademicYearId
    LEFT JOIN Faculties f ON f.Id = s.ClassTeacherId
    LEFT JOIN Rooms r ON r.RoomId = s.RoomId
    WHERE s.SectionId = p_SectionId;
END;

-- 4. sp_CreateSection
DROP PROCEDURE IF EXISTS sp_CreateSection;
CREATE PROCEDURE sp_CreateSection(
    IN p_Board VARCHAR(100),
    IN p_BoardId INT,
    IN p_AcademicYearId INT,
    IN p_Group VARCHAR(100),
    IN p_GroupId INT,
    IN p_Programme VARCHAR(100),
    IN p_AcademicLevel VARCHAR(50),
    IN p_SectionName VARCHAR(50),
    IN p_RoomNumber VARCHAR(50),
    IN p_ClassTeacherId INT,
    IN p_MaximumStrength INT,
    IN p_IsActive TINYINT(1),
    IN p_RoomId INT
)
BEGIN
    INSERT INTO Sections (
        Board, BoardId, AcademicYearId, `Group`, GroupId, Programme, AcademicLevel, 
        SectionName, RoomNumber, ClassTeacherId, MaximumStrength, IsActive, RoomId, CreatedAt
    )
    VALUES (
        p_Board, p_BoardId, p_AcademicYearId, p_Group, p_GroupId, COALESCE(p_Programme, ''), p_AcademicLevel, 
        p_SectionName, p_RoomNumber, p_ClassTeacherId, p_MaximumStrength, p_IsActive, p_RoomId, UTC_TIMESTAMP()
    );
    SELECT LAST_INSERT_ID();
END;

-- 5. sp_UpdateSection
DROP PROCEDURE IF EXISTS sp_UpdateSection;
CREATE PROCEDURE sp_UpdateSection(
    IN p_SectionId INT,
    IN p_Board VARCHAR(100),
    IN p_BoardId INT,
    IN p_AcademicYearId INT,
    IN p_Group VARCHAR(100),
    IN p_GroupId INT,
    IN p_Programme VARCHAR(100),
    IN p_AcademicLevel VARCHAR(50),
    IN p_SectionName VARCHAR(50),
    IN p_RoomNumber VARCHAR(50),
    IN p_ClassTeacherId INT,
    IN p_MaximumStrength INT,
    IN p_IsActive TINYINT(1),
    IN p_RoomId INT
)
BEGIN
    UPDATE Sections
    SET Board = p_Board,
        BoardId = COALESCE(p_BoardId, BoardId),
        AcademicYearId = p_AcademicYearId,
        `Group` = p_Group,
        GroupId = COALESCE(p_GroupId, GroupId),
        Programme = COALESCE(p_Programme, ''),
        AcademicLevel = p_AcademicLevel,
        SectionName = p_SectionName,
        RoomNumber = p_RoomNumber,
        ClassTeacherId = p_ClassTeacherId,
        MaximumStrength = p_MaximumStrength,
        IsActive = p_IsActive,
        RoomId = p_RoomId,
        UpdatedAt = UTC_TIMESTAMP()
    WHERE SectionId = p_SectionId;
END;

-- 6. sp_DeleteSection
DROP PROCEDURE IF EXISTS sp_DeleteSection;
CREATE PROCEDURE sp_DeleteSection(IN p_SectionId INT)
BEGIN
    DELETE FROM Sections WHERE SectionId = p_SectionId;
END;

-- 7. sp_ValidateSectionName
DROP PROCEDURE IF EXISTS sp_ValidateSectionName;
CREATE PROCEDURE sp_ValidateSectionName(
    IN p_Board VARCHAR(100),
    IN p_AcademicYearId INT,
    IN p_Group VARCHAR(100),
    IN p_Programme VARCHAR(100),
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
      AND (Programme = p_Programme OR (Programme IS NULL AND p_Programme = '') OR (Programme = '' AND p_Programme IS NULL))
      AND AcademicLevel = p_AcademicLevel
      AND SectionName = p_SectionName
      AND (p_ExcludeSectionId IS NULL OR SectionId <> p_ExcludeSectionId);
END;

-- 8. sp_GetSectionsByGroup
DROP PROCEDURE IF EXISTS sp_GetSectionsByGroup;
CREATE PROCEDURE sp_GetSectionsByGroup(IN p_GroupId INT)
BEGIN
    SELECT s.SectionId,
           s.BoardId,
           s.Board,
           s.AcademicYearId,
           COALESCE(ay.AcademicYearName, '') AS AcademicYearName,
           s.GroupId,
           s.`Group`,
           COALESCE(s.Programme, '') AS Programme,
           s.AcademicLevel,
           s.SectionName,
           s.RoomNumber,
           s.RoomId,
           COALESCE(r.RoomName, s.RoomNumber, '') AS RoomName,
           s.ClassTeacherId,
           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS ClassTeacherName,
           s.MaximumStrength,
           s.IsActive,
           s.CreatedAt,
           s.UpdatedAt
    FROM Sections s
    LEFT JOIN AcademicYears ay ON ay.AcademicYearId = s.AcademicYearId
    LEFT JOIN Faculties f ON f.Id = s.ClassTeacherId
    LEFT JOIN Rooms r ON r.RoomId = s.RoomId
    WHERE s.GroupId = p_GroupId 
       OR s.`Group` = (SELECT GroupName FROM `Groups` WHERE GroupId = p_GroupId LIMIT 1)
    ORDER BY s.SectionName ASC;
END;
", suppressTransaction: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
