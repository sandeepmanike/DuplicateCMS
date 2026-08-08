DROP PROCEDURE IF EXISTS sp_GetStudentsForAttendance;

DELIMITER $$

-- =================================================================================
-- Author:      Senior MySQL 8 Database Architect
-- Purpose:     Retrieves students available to mark attendance, joining their 
--              current attendance status if already recorded for the selected date.
-- Input:       p_BoardId, p_AcademicYearId, p_AcademicLevelId, p_GroupId,
--              p_SectionId, p_SubjectId, p_FacultyId - Filters
--              p_FromDate - The selected attendance date to check status
-- Return:      A result set matching StudentAttendanceResponse.
-- =================================================================================
CREATE PROCEDURE sp_GetStudentsForAttendance(
    IN p_BoardId INT,
    IN p_AcademicYearId INT,
    IN p_AcademicLevelId INT,
    IN p_GroupId INT,
    IN p_SectionId INT,
    IN p_SubjectId INT,
    IN p_FacultyId INT,
    IN p_StudentId INT,
    IN p_Status TINYINT,
    IN p_FromDate DATETIME,
    IN p_ToDate DATETIME,
    IN p_PageNumber INT,
    IN p_PageSize INT,
    IN p_SearchText VARCHAR(100)
)
BEGIN
    SELECT 
        s.StudentId,
        COALESCE(s.AdmissionNumber, '') AS AdmissionNumber,
        COALESCE(s.RollNumber, '') AS RollNumber,
        COALESCE(s.StudentName, '') AS StudentName,
        COALESCE(a.Status, 0) AS Status,
        COALESCE(a.Remarks,'') AS Remarks,
        (CASE WHEN a.AttendanceId IS NOT NULL THEN 1 ELSE 0 END) AS IsAttendanceMarked
    FROM Students s
    LEFT JOIN Attendances a ON s.StudentId = a.StudentId 
                          AND a.SubjectId = p_SubjectId 
                          AND DATE(a.AttendanceDate) = DATE(p_FromDate)
                          AND a.IsActive = 1
    WHERE s.IsActive = 1
      AND (p_BoardId IS NULL OR p_BoardId = 0 OR s.BoardId = p_BoardId)
      AND (p_AcademicYearId IS NULL OR p_AcademicYearId = 0 OR s.AcademicYearId = p_AcademicYearId)
      AND (p_AcademicLevelId IS NULL OR p_AcademicLevelId = 0 OR s.AcademicLevelId = p_AcademicLevelId)
      AND (p_GroupId IS NULL OR p_GroupId = 0 OR s.GroupId = p_GroupId)
      AND (p_SectionId IS NULL OR p_SectionId = 0 OR s.SectionId = p_SectionId)
      AND (p_StudentId IS NULL OR p_StudentId = 0 OR s.StudentId = p_StudentId)
      AND (p_SearchText IS NULL OR p_SearchText = '' OR 
           s.StudentName LIKE CONCAT('%', p_SearchText, '%') OR 
           s.RollNumber LIKE CONCAT('%', p_SearchText, '%') OR
           s.AdmissionNumber LIKE CONCAT('%', p_SearchText, '%'))
    ORDER BY s.RollNumber ASC, s.StudentName ASC;
END$$

DELIMITER ;
