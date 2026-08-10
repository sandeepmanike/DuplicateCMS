DROP PROCEDURE IF EXISTS sp_CreateAttendance;

DELIMITER $$

-- =================================================================================
-- Author:      Senior MySQL 8 Database Architect
-- Purpose:     Create a new student attendance record in the Attendances table.
-- Input:       p_AttendanceDate - Date and time of the attendance
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
-- Return:      The unique identifier of the newly created attendance record.
-- =================================================================================
CREATE PROCEDURE sp_CreateAttendance(
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

    INSERT INTO Attendances (
        AttendanceDate, 
        StudentId, 
        FacultyId, 
        BoardId, 
        AcademicYearId, 
        AcademicLevelId, 
        GroupId, 
        SectionId, 
        SubjectId, 
        Status, 
        Remarks
      
    ) VALUES (
        p_AttendanceDate, 
        p_StudentId, 
        p_FacultyId, 
        p_BoardId, 
        p_AcademicYearId, 
        p_AcademicLevelId, 
        p_GroupId, 
        p_SectionId, 
        p_SubjectId, 
        p_Status, 
        p_Remarks, 
        1, -- IsActive defaults to true on creation
        UTC_TIMESTAMP()
    );

    COMMIT;

    SELECT LAST_INSERT_ID() AS AttendanceId;
END$$

DELIMITER ;
