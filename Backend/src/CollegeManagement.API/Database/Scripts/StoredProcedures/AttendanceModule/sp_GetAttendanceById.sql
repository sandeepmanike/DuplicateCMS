DROP PROCEDURE IF EXISTS sp_GetAttendanceById;

DELIMITER $$

-- =================================================================================
-- Author:      Senior MySQL 8 Database Architect
-- Purpose:     Retrieve detailed attendance response by unique identifier.
-- Input:       p_AttendanceId - Unique identifier of the attendance record
-- Return:      A result set representing the attendance details.
-- =================================================================================
CREATE PROCEDURE sp_GetAttendanceById(
    IN p_AttendanceId INT
)
BEGIN
    SELECT 
        a.AttendanceId,
        a.AttendanceDate,
        a.StudentId,
        COALESCE(s.StudentName, '') AS StudentName,
        COALESCE(s.RollNumber, '') AS RollNumber,
        a.FacultyId,
        TRIM(CONCAT(COALESCE(f.FirstName, ''), ' ', COALESCE(f.LastName, ''))) AS FacultyName,
        a.BoardId,
        COALESCE(b.BoardName, '') AS BoardName,
        a.AcademicYearId,
        COALESCE(ay.AcademicYearName, '') AS AcademicYearName,
        a.AcademicLevelId,
        COALESCE(al.LevelName, '') AS AcademicLevelName,
        a.GroupId,
        COALESCE(g.GroupName, '') AS GroupName,
        a.SectionId,
        COALESCE(sec.SectionName, '') AS SectionName,
        a.SubjectId,
        COALESCE(sub.SubjectName, '') AS SubjectName,
        a.Status,
        a.Remarks,
        a.CreatedAt,
        a.UpdatedAt
    FROM Attendances a
    INNER JOIN Students s ON a.StudentId = s.StudentId
    INNER JOIN Faculties f ON a.FacultyId = f.Id
    INNER JOIN Boards b ON a.BoardId = b.BoardId
    INNER JOIN AcademicYears ay ON a.AcademicYearId = ay.AcademicYearId
    INNER JOIN AcademicLevels al ON a.AcademicLevelId = al.AcademicLevelId
    INNER JOIN Groups g ON a.GroupId = g.GroupId
    INNER JOIN Sections sec ON a.SectionId = sec.SectionId
    INNER JOIN Subjects sub ON a.SubjectId = sub.SubjectId
    WHERE a.AttendanceId = p_AttendanceId;
END$$

DELIMITER ;
