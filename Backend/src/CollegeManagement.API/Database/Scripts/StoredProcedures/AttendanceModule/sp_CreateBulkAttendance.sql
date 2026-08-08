DROP PROCEDURE IF EXISTS sp_CreateBulkAttendance;

DELIMITER $$

-- =================================================================================
-- Author:      Senior MySQL 8 Database Architect
-- Purpose:     Bulk insert attendance records using a JSON array input.
-- Input:       p_AttendanceJson - JSON array containing attendance records
-- Return:      The number of records successfully inserted.
-- =================================================================================
CREATE PROCEDURE sp_CreateBulkAttendance(
    IN p_AttendanceJson JSON
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
        Remarks, 
        IsActive, 
        CreatedAt
    )
    SELECT 
        jt.AttendanceDate, 
        jt.StudentId, 
        jt.FacultyId, 
        jt.BoardId, 
        jt.AcademicYearId, 
        jt.AcademicLevelId, 
        jt.GroupId, 
        jt.SectionId, 
        jt.SubjectId, 
        jt.Status, 
        jt.Remarks, 
        IFNULL(jt.IsActive, 1), 
        UTC_TIMESTAMP()
    FROM JSON_TABLE(
        p_AttendanceJson,
        '$[*]' COLUMNS(
            AttendanceDate DATETIME PATH '$.AttendanceDate',
            StudentId INT PATH '$.StudentId',
            FacultyId INT PATH '$.FacultyId',
            BoardId INT PATH '$.BoardId',
            AcademicYearId INT PATH '$.AcademicYearId',
            AcademicLevelId INT PATH '$.AcademicLevelId',
            GroupId INT PATH '$.GroupId',
            SectionId INT PATH '$.SectionId',
            SubjectId INT PATH '$.SubjectId',
            Status TINYINT PATH '$.Status',
            Remarks VARCHAR(500) PATH '$.Remarks',
            IsActive BOOLEAN PATH '$.IsActive'
        )
    ) jt;

    COMMIT;

    SELECT ROW_COUNT() AS AffectedRows;
END$$

DELIMITER ;
