-- =============================================================================
-- FIX SECTIONS TABLE SCHEMA & STORED PROCEDURES
-- DATABASE: u819242402_CLM_System
-- =============================================================================

USE `u819242402_CLM_System`;

-- 1. Safely add missing columns to Sections table if they do not exist
SET @exist := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Sections' AND COLUMN_NAME = 'Board');
SET @sqlstmt := IF(@exist = 0, 'ALTER TABLE `Sections` ADD COLUMN `Board` VARCHAR(100) NOT NULL DEFAULT \'\' AFTER `SectionId`', 'SELECT 1');
PREPARE stmt FROM @sqlstmt; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @exist := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Sections' AND COLUMN_NAME = 'AcademicYearId');
SET @sqlstmt := IF(@exist = 0, 'ALTER TABLE `Sections` ADD COLUMN `AcademicYearId` INT NOT NULL DEFAULT 1 AFTER `Board`', 'SELECT 1');
PREPARE stmt FROM @sqlstmt; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @exist := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Sections' AND COLUMN_NAME = 'Group');
SET @sqlstmt := IF(@exist = 0, 'ALTER TABLE `Sections` ADD COLUMN `Group` VARCHAR(100) NOT NULL DEFAULT \'\' AFTER `AcademicYearId`', 'SELECT 1');
PREPARE stmt FROM @sqlstmt; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @exist := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Sections' AND COLUMN_NAME = 'AcademicLevel');
SET @sqlstmt := IF(@exist = 0, 'ALTER TABLE `Sections` ADD COLUMN `AcademicLevel` VARCHAR(50) NOT NULL DEFAULT \'\' AFTER `Group`', 'SELECT 1');
PREPARE stmt FROM @sqlstmt; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @exist := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Sections' AND COLUMN_NAME = 'RoomNumber');
SET @sqlstmt := IF(@exist = 0, 'ALTER TABLE `Sections` ADD COLUMN `RoomNumber` VARCHAR(50) NULL AFTER `SectionName`', 'SELECT 1');
PREPARE stmt FROM @sqlstmt; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @exist := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Sections' AND COLUMN_NAME = 'ClassTeacherId');
SET @sqlstmt := IF(@exist = 0, 'ALTER TABLE `Sections` ADD COLUMN `ClassTeacherId` INT NULL AFTER `RoomNumber`', 'SELECT 1');
PREPARE stmt FROM @sqlstmt; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @exist := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Sections' AND COLUMN_NAME = 'MaximumStrength');
SET @sqlstmt := IF(@exist = 0, 'ALTER TABLE `Sections` ADD COLUMN `MaximumStrength` INT NOT NULL DEFAULT 60 AFTER `ClassTeacherId`', 'SELECT 1');
PREPARE stmt FROM @sqlstmt; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- 2. sp_GetAllSections
DROP PROCEDURE IF EXISTS sp_GetAllSections;
DELIMITER //
CREATE PROCEDURE sp_GetAllSections()
BEGIN
    SELECT s.SectionId, s.Board, s.AcademicYearId, ay.AcademicYearName, s.`Group`, s.AcademicLevel,
           s.SectionName, s.RoomNumber, s.ClassTeacherId, 
           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS ClassTeacherName,
           s.MaximumStrength, s.IsActive, s.CreatedAt, s.UpdatedAt
    FROM Sections s
    LEFT JOIN AcademicYears ay ON ay.AcademicYearId = s.AcademicYearId
    LEFT JOIN Faculties f ON f.Id = s.ClassTeacherId
    ORDER BY s.SectionId DESC;
END //
DELIMITER ;

-- 3. sp_GetSectionById
DROP PROCEDURE IF EXISTS sp_GetSectionById;
DELIMITER //
CREATE PROCEDURE sp_GetSectionById(IN p_SectionId INT)
BEGIN
    SELECT s.SectionId, s.Board, s.AcademicYearId, ay.AcademicYearName, s.`Group`, s.AcademicLevel,
           s.SectionName, s.RoomNumber, s.ClassTeacherId, 
           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS ClassTeacherName,
           s.MaximumStrength, s.IsActive, s.CreatedAt, s.UpdatedAt
    FROM Sections s
    LEFT JOIN AcademicYears ay ON ay.AcademicYearId = s.AcademicYearId
    LEFT JOIN Faculties f ON f.Id = s.ClassTeacherId
    WHERE s.SectionId = p_SectionId;
END //
DELIMITER ;

-- 4. sp_CreateSection
DROP PROCEDURE IF EXISTS sp_CreateSection;
DELIMITER //
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
END //
DELIMITER ;

-- 5. sp_UpdateSection
DROP PROCEDURE IF EXISTS sp_UpdateSection;
DELIMITER //
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
END //
DELIMITER ;

-- 6. sp_DeleteSection
DROP PROCEDURE IF EXISTS sp_DeleteSection;
DELIMITER //
CREATE PROCEDURE sp_DeleteSection(IN p_SectionId INT)
BEGIN
    DELETE FROM Sections WHERE SectionId = p_SectionId;
END //
DELIMITER ;

-- 7. sp_ValidateSectionName
DROP PROCEDURE IF EXISTS sp_ValidateSectionName;
DELIMITER //
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
      AND SectionName = p_SectionName
      AND (p_ExcludeSectionId IS NULL OR SectionId <> p_ExcludeSectionId);
END //
DELIMITER ;
