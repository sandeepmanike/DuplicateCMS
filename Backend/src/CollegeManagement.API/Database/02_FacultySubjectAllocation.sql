-- =============================================================================
-- MODULE: FACULTY SUBJECT ALLOCATION
-- DATABASE: u819242402_CLM_System
-- DESCRIPTION: Contains all MySQL Stored Procedures for Faculty Subject Allocation
-- =============================================================================

USE u819242402_CLM_System;

-- -----------------------------------------------------------------------------
-- 1. sp_GetSubjectAllocationById
-- -----------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS sp_GetSubjectAllocationById;
DELIMITER //
CREATE PROCEDURE sp_GetSubjectAllocationById(
    IN p_Id INT
)
BEGIN
    SELECT 
        fsa.Id,
        fsa.FacultyId,
        fsa.BoardId,
        fsa.AcademicLevelId,
        fsa.AcademicYearId,
        fsa.GroupId,
        fsa.SectionId,
        fsa.SubjectId,
        fsa.CreatedAt,
        fsa.UpdatedAt,

        f.Id,
        f.EmployeeId,
        f.FirstName,
        f.LastName,
        f.Email,

        b.BoardId,
        b.BoardCode,
        b.BoardName,

        al.AcademicLevelId,
        al.LevelCode,
        al.LevelName,

        ay.AcademicYearId,
        ay.AcademicYearName,

        g.GroupId,
        g.GroupCode,
        g.GroupName,

        sec.SectionId,
        sec.SectionName,

        sub.SubjectId,
        sub.SubjectCode,
        sub.SubjectName
    FROM FacultySubjectAllocations fsa
    INNER JOIN Faculties f ON f.Id = fsa.FacultyId
    INNER JOIN Boards b ON b.BoardId = fsa.BoardId
    INNER JOIN AcademicLevels al ON al.AcademicLevelId = fsa.AcademicLevelId
    INNER JOIN AcademicYears ay ON ay.AcademicYearId = fsa.AcademicYearId
    INNER JOIN `Groups` g ON g.GroupId = fsa.GroupId
    INNER JOIN Sections sec ON sec.SectionId = fsa.SectionId
    INNER JOIN Subjects sub ON sub.SubjectId = fsa.SubjectId
    WHERE fsa.Id = p_Id;
END //
DELIMITER ;

-- -----------------------------------------------------------------------------
-- 2. sp_GetSubjectAllocationsByFacultyId
-- -----------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS sp_GetSubjectAllocationsByFacultyId;
DELIMITER //
CREATE PROCEDURE sp_GetSubjectAllocationsByFacultyId(
    IN p_FacultyId INT
)
BEGIN
    SELECT 
        fsa.Id,
        fsa.FacultyId,
        fsa.BoardId,
        fsa.AcademicLevelId,
        fsa.AcademicYearId,
        fsa.GroupId,
        fsa.SectionId,
        fsa.SubjectId,
        fsa.CreatedAt,
        fsa.UpdatedAt,

        f.Id,
        f.EmployeeId,
        f.FirstName,
        f.LastName,
        f.Email,

        b.BoardId,
        b.BoardCode,
        b.BoardName,

        al.AcademicLevelId,
        al.LevelCode,
        al.LevelName,

        ay.AcademicYearId,
        ay.AcademicYearName,

        g.GroupId,
        g.GroupCode,
        g.GroupName,

        sec.SectionId,
        sec.SectionName,

        sub.SubjectId,
        sub.SubjectCode,
        sub.SubjectName
    FROM FacultySubjectAllocations fsa
    INNER JOIN Faculties f ON f.Id = fsa.FacultyId
    INNER JOIN Boards b ON b.BoardId = fsa.BoardId
    INNER JOIN AcademicLevels al ON al.AcademicLevelId = fsa.AcademicLevelId
    INNER JOIN AcademicYears ay ON ay.AcademicYearId = fsa.AcademicYearId
    INNER JOIN `Groups` g ON g.GroupId = fsa.GroupId
    INNER JOIN Sections sec ON sec.SectionId = fsa.SectionId
    INNER JOIN Subjects sub ON sub.SubjectId = fsa.SubjectId
    WHERE fsa.FacultyId = p_FacultyId
    ORDER BY fsa.Id DESC;
END //
DELIMITER ;

-- -----------------------------------------------------------------------------
-- 3. sp_CreateSubjectAllocation
-- -----------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS sp_CreateSubjectAllocation;
DELIMITER //
CREATE PROCEDURE sp_CreateSubjectAllocation(
    IN p_FacultyId INT,
    IN p_BoardId INT,
    IN p_AcademicLevelId INT,
    IN p_AcademicYearId INT,
    IN p_GroupId INT,
    IN p_SectionId INT,
    IN p_SubjectId INT
)
BEGIN
    INSERT INTO FacultySubjectAllocations (
        FacultyId, BoardId, AcademicLevelId, AcademicYearId, GroupId, SectionId, SubjectId, CreatedAt
    ) VALUES (
        p_FacultyId, p_BoardId, p_AcademicLevelId, p_AcademicYearId, p_GroupId, p_SectionId, p_SubjectId, NOW()
    );
    SELECT LAST_INSERT_ID() AS Id;
END //
DELIMITER ;

-- -----------------------------------------------------------------------------
-- 4. sp_UpdateSubjectAllocation
-- -----------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS sp_UpdateSubjectAllocation;
DELIMITER //
CREATE PROCEDURE sp_UpdateSubjectAllocation(
    IN p_Id INT,
    IN p_BoardId INT,
    IN p_AcademicLevelId INT,
    IN p_AcademicYearId INT,
    IN p_GroupId INT,
    IN p_SectionId INT,
    IN p_SubjectId INT
)
BEGIN
    UPDATE FacultySubjectAllocations SET
        BoardId = p_BoardId,
        AcademicLevelId = p_AcademicLevelId,
        AcademicYearId = p_AcademicYearId,
        GroupId = p_GroupId,
        SectionId = p_SectionId,
        SubjectId = p_SubjectId,
        UpdatedAt = NOW()
    WHERE Id = p_Id;
END //
DELIMITER ;

-- -----------------------------------------------------------------------------
-- 5. sp_DeleteSubjectAllocation
-- -----------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS sp_DeleteSubjectAllocation;
DELIMITER //
CREATE PROCEDURE sp_DeleteSubjectAllocation(
    IN p_Id INT
)
BEGIN
    DELETE FROM FacultySubjectAllocations WHERE Id = p_Id;
END //
DELIMITER ;

-- -----------------------------------------------------------------------------
-- 6. sp_CheckDuplicateSubjectAllocation
-- -----------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS sp_CheckDuplicateSubjectAllocation;
DELIMITER //
CREATE PROCEDURE sp_CheckDuplicateSubjectAllocation(
    IN p_FacultyId INT,
    IN p_BoardId INT,
    IN p_AcademicLevelId INT,
    IN p_AcademicYearId INT,
    IN p_GroupId INT,
    IN p_SectionId INT,
    IN p_SubjectId INT,
    IN p_ExcludeId INT
)
BEGIN
    SELECT COUNT(*) 
    FROM FacultySubjectAllocations
    WHERE FacultyId = p_FacultyId
      AND BoardId = p_BoardId
      AND AcademicLevelId = p_AcademicLevelId
      AND AcademicYearId = p_AcademicYearId
      AND GroupId = p_GroupId
      AND SectionId = p_SectionId
      AND SubjectId = p_SubjectId
      AND (p_ExcludeId IS NULL OR Id <> p_ExcludeId);
END //
DELIMITER ;
