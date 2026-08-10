DROP PROCEDURE IF EXISTS sp_AttendanceExists;

DELIMITER $$

-- =================================================================================
-- Author:      Senior MySQL 8 Database Architect
-- Purpose:     Check if an active attendance record already exists for the given 
--              StudentId, SubjectId, and AttendanceDate.
-- Input:       p_StudentId - The unique identifier of the student
--              p_SubjectId - The unique identifier of the subject
--              p_AttendanceDate - The date and time of the attendance
-- Return:      Returns 1 (TRUE) if record exists, otherwise 0 (FALSE).
-- =================================================================================
CREATE PROCEDURE sp_AttendanceExists(
    IN p_StudentId INT,
    IN p_SubjectId INT,
    IN p_AttendanceDate DATETIME
)
BEGIN
    SELECT EXISTS (
        SELECT 1 
        FROM Attendances 
        WHERE StudentId = p_StudentId 
          AND SubjectId = p_SubjectId 
        AND DATE(AttendanceDate) = DATE(p_AttendanceDate)
          AND IsActive = 1
    ) AS AttendanceExists;
END$$

DELIMITER ;
