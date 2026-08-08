DROP PROCEDURE IF EXISTS sp_GetAttendances;

DELIMITER $$

-- =================================================================================
-- Author:      Senior MySQL 8 Database Architect
-- Purpose:     Retrieve a filtered, paginated list of active attendance records.
-- Input:       p_BoardId, p_AcademicYearId, p_AcademicLevelId, p_GroupId,
--              p_SectionId, p_SubjectId, p_FacultyId, p_StudentId - Filters
--              p_Status - Status filter (1 = Present, 2 = Absent, 3 = Late, 4 = Leave)
--              p_FromDate, p_ToDate - Date range filters
--              p_PageNumber, p_PageSize - Pagination parameters
--              p_SearchText - Search query for student name/roll number/faculty name
-- Return:      A filtered result set matching AttendanceListResponse.
-- =================================================================================
CREATE PROCEDURE sp_GetAttendances(
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
    DECLARE v_Offset INT;
    DECLARE v_Limit INT;

    SET v_Limit = IFNULL(p_PageSize, 10);
    IF v_Limit <= 0 THEN 
        SET v_Limit = 10; 
    END IF;

    IF p_PageNumber IS NULL OR p_PageNumber <= 0 THEN
        SET v_Offset = 0;
    ELSE
        SET v_Offset = (p_PageNumber - 1) * v_Limit;
    END IF;

    SELECT 
        a.AttendanceId,
        a.AttendanceDate,
        a.StudentId,
        COALESCE(s.RollNumber, '') AS RollNumber,
        COALESCE(s.StudentName, '') AS StudentName,
        TRIM(CONCAT(COALESCE(f.FirstName, ''), ' ', COALESCE(f.LastName, ''))) AS FacultyName,
        COALESCE(sub.SubjectName, '') AS SubjectName,
        a.Status
    FROM Attendances a
    INNER JOIN Students s ON a.StudentId = s.StudentId
    INNER JOIN Faculties f ON a.FacultyId = f.Id
    INNER JOIN Subjects sub ON a.SubjectId = sub.SubjectId
    WHERE a.IsActive = 1
      AND (p_BoardId IS NULL OR p_BoardId = 0 OR a.BoardId = p_BoardId)
      AND (p_AcademicYearId IS NULL OR p_AcademicYearId = 0 OR a.AcademicYearId = p_AcademicYearId)
      AND (p_AcademicLevelId IS NULL OR p_AcademicLevelId = 0 OR a.AcademicLevelId = p_AcademicLevelId)
      AND (p_GroupId IS NULL OR p_GroupId = 0 OR a.GroupId = p_GroupId)
      AND (p_SectionId IS NULL OR p_SectionId = 0 OR a.SectionId = p_SectionId)
      AND (p_SubjectId IS NULL OR p_SubjectId = 0 OR a.SubjectId = p_SubjectId)
      AND (p_FacultyId IS NULL OR p_FacultyId = 0 OR a.FacultyId = p_FacultyId)
      AND (p_StudentId IS NULL OR p_StudentId = 0 OR a.StudentId = p_StudentId)
      AND (p_Status IS NULL OR a.Status = p_Status)
      AND (p_FromDate IS NULL OR DATE(a.AttendanceDate) >= DATE(p_FromDate))
      AND (p_ToDate IS NULL OR DATE(a.AttendanceDate) <= DATE(p_ToDate))
      AND (p_SearchText IS NULL OR p_SearchText = '' OR 
           s.StudentName LIKE CONCAT('%', p_SearchText, '%') OR 
           s.RollNumber LIKE CONCAT('%', p_SearchText, '%') OR 
           CONCAT(f.FirstName,' ',f.LastName) LIKE CONCAT('%',p_SearchText,'%'))
    ORDER BY a.AttendanceDate DESC, a.AttendanceId DESC
    LIMIT v_Limit OFFSET v_Offset;
END$$

DELIMITER ;
