DROP PROCEDURE IF EXISTS sp_GetAttendanceReport;

DELIMITER $$

-- =================================================================================
-- Author:      Senior MySQL 8 Database Architect
-- Purpose:     Generate flat report listing attendance details with all lookup descriptors.
-- Input:       Filters (BoardId, AcademicYearId, AcademicLevelId, GroupId, SectionId, 
--              SubjectId, FacultyId, StudentId, Status, Date range, Search text)
-- Return:      A collection of attendance report entries.
-- =================================================================================
CREATE PROCEDURE sp_GetAttendanceReport(
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
    
        a.AttendanceDate,
        COALESCE(b.BoardName, '') AS BoardName,
        COALESCE(ay.AcademicYearName, '') AS AcademicYearName,
        COALESCE(al.LevelName, '') AS AcademicLevelName,
        COALESCE(g.GroupName, '') AS GroupName,
        COALESCE(sec.SectionName, '') AS SectionName,
        COALESCE(sub.SubjectName, '') AS SubjectName,
        TRIM(CONCAT(COALESCE(f.FirstName, ''), ' ', COALESCE(f.LastName, ''))) AS FacultyName,
        COALESCE(s.RollNumber, '') AS RollNumber,
        COALESCE(s.StudentName, '') AS StudentName,
        a.Status,
        a.Remarks
    FROM Attendances a
    INNER JOIN Students s ON a.StudentId = s.StudentId
    INNER JOIN Faculties f ON a.FacultyId = f.Id
    INNER JOIN Boards b ON a.BoardId = b.BoardId
    INNER JOIN AcademicYears ay ON a.AcademicYearId = ay.AcademicYearId
    INNER JOIN AcademicLevels al ON a.AcademicLevelId = al.AcademicLevelId
    INNER JOIN Groups g ON a.GroupId = g.GroupId
    INNER JOIN Sections sec ON a.SectionId = sec.SectionId
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
           CONCAT(f.FirstName,' ',f.LastName) LIKE CONCAT('%', p_SearchText, '%'))
    ORDER BY a.AttendanceDate DESC, s.RollNumber ASC;
END$$

DELIMITER ;
