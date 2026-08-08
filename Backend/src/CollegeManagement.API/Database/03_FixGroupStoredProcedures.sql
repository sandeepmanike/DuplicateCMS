-- =============================================================================
-- FIX GROUP STORED PROCEDURES & RESULT SETS
-- DATABASE: u819242402_CLM_System
-- =============================================================================

USE `u819242402_CLM_System`;

DROP PROCEDURE IF EXISTS sp_GetAllGroups;
DELIMITER //
CREATE PROCEDURE sp_GetAllGroups(
    IN p_PageNumber INT,
    IN p_PageSize INT,
    IN p_Search VARCHAR(100),
    IN p_Board VARCHAR(100),
    IN p_AcademicYearId INT,
    IN p_AcademicLevel VARCHAR(50),
    IN p_IsActive TINYINT(1)
)
BEGIN
    DECLARE v_Offset INT;
    SET v_Offset = (IFNULL(p_PageNumber, 1) - 1) * IFNULL(p_PageSize, 10);

    -- Result Set 1: Filtered & Paginated Groups
    SELECT
        g.GroupId,
        g.Board,
        g.AcademicYearId,
        COALESCE(ay.AcademicYearName, '') AS AcademicYearName,
        g.AcademicLevel,
        g.GroupName,
        g.GroupCode,
        g.Description,
        0 AS TotalSubjects,
        g.IsActive,
        CASE
            WHEN g.IsActive = 1 THEN 'Active'
            ELSE 'Inactive'
        END AS Status,
        g.CreatedAt,
        g.UpdatedAt
    FROM `Groups` g
    LEFT JOIN AcademicYears ay ON ay.AcademicYearId = g.AcademicYearId
    WHERE (p_Search IS NULL OR g.GroupName LIKE CONCAT('%', p_Search, '%') OR g.GroupCode LIKE CONCAT('%', p_Search, '%'))
      AND (p_Board IS NULL OR g.Board = p_Board)
      AND (p_AcademicYearId IS NULL OR g.AcademicYearId = p_AcademicYearId)
      AND (p_AcademicLevel IS NULL OR g.AcademicLevel = p_AcademicLevel)
      AND (p_IsActive IS NULL OR g.IsActive = p_IsActive)
    ORDER BY g.GroupId DESC
    LIMIT v_Offset, p_PageSize;

    -- Result Set 2: Total Count of Matching Records
    SELECT COUNT(*) AS TotalCount
    FROM `Groups` g
    WHERE (p_Search IS NULL OR g.GroupName LIKE CONCAT('%', p_Search, '%') OR g.GroupCode LIKE CONCAT('%', p_Search, '%'))
      AND (p_Board IS NULL OR g.Board = p_Board)
      AND (p_AcademicYearId IS NULL OR g.AcademicYearId = p_AcademicYearId)
      AND (p_AcademicLevel IS NULL OR g.AcademicLevel = p_AcademicLevel)
      AND (p_IsActive IS NULL OR g.IsActive = p_IsActive);
END //
DELIMITER ;
