DROP PROCEDURE IF EXISTS sp_UpdateAttendance;

DELIMITER $$

-- =================================================================================
-- Author:      Senior MySQL 8 Database Architect
-- Purpose:     Update an existing attendance record in the database.
-- Input:       p_AttendanceId - Unique identifier of the attendance record to update
--              p_AttendanceDate - New attendance date and time
--              p_StudentId - Student identifier
--              p_FacultyId - Faculty member identifier
--              p_BoardId - Board identifier
--              p_AcademicYearId - Academic year identifier
--              p_AcademicLevelId - Academic level identifier
--              p_GroupId - Group identifier
--              p_SectionId - Section identifier
--              p_SubjectId - Subject identifier
--              p_Status - Attendance status (1 = Present, 2 = Absent, 3 = Late, 4 = Leave)
--              p_Remarks - Remarks/notes for the attendance record
-- Return:      The number of records successfully updated.
-- =================================================================================
CREATE PROCEDURE sp_UpdateAttendance(
    IN p_AttendanceId INT,
    IN p_AttendanceDate DATETIME,
    IN p_StudentId INT,
    IN p_FacultyId INT,
    IN p_BoardId INT,
    IN p_AcademicYearId INT,
    IN p_AcademicLevelId INT,
    IN p_GroupId INT,
    IN p_SectionId INT,
    IN p_SubjectId INT,
    IN p_Status TINYINT,
    IN p_Remarks VARCHAR(500)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;

    UPDATE Attendances
    SET AttendanceDate = p_AttendanceDate,
        StudentId = p_StudentId,
        FacultyId = p_FacultyId,
        BoardId = p_BoardId,
        AcademicYearId = p_AcademicYearId,
        AcademicLevelId = p_AcademicLevelId,
        GroupId = p_GroupId,
        SectionId = p_SectionId,
        SubjectId = p_SubjectId,
        Status = p_Status,
        Remarks = p_Remarks,
        UpdatedAt = UTC_TIMESTAMP()
    WHERE AttendanceId = p_AttendanceId;

    COMMIT;

    SELECT ROW_COUNT() AS AffectedRows;
END$$

DELIMITER ;
