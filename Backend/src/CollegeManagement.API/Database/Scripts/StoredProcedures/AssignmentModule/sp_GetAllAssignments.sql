DROP PROCEDURE IF EXISTS sp_GetAllAssignments;
DELIMITER //
CREATE PROCEDURE sp_GetAllAssignments()
BEGIN
    SELECT 
        AssignmentId,
        Title,
        SubjectId,
        FacultyId,
        AcademicYearId,
        AcademicLevel,
        Description,
        DueDate,
        Attachment,
        MaximumMarks
    FROM Assignments
    ORDER BY AssignmentId DESC;
END //
DELIMITER ;
