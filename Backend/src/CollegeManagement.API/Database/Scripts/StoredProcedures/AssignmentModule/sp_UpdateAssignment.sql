DROP PROCEDURE IF EXISTS sp_UpdateAssignment;

DELIMITER $$

CREATE PROCEDURE sp_UpdateAssignment
(
    IN p_AssignmentId INT,
    IN p_Title VARCHAR(200),
    IN p_AcademicYearId INT,
    IN p_AcademicLevel VARCHAR(50),
    IN p_GroupId INT,
    IN p_SubjectId INT,
    IN p_FacultyId INT,
    IN p_Description VARCHAR(1000),
    IN p_DueDate DATE,
    IN p_Attachment VARCHAR(500),
    IN p_MaximumMarks INT
)
BEGIN

UPDATE Assignments
SET
Title=p_Title,
AcademicYearId=p_AcademicYearId,
AcademicLevel=p_AcademicLevel,
GroupId=p_GroupId,
SubjectId=p_SubjectId,
FacultyId=p_FacultyId,
Description=p_Description,
DueDate=p_DueDate,
Attachment=p_Attachment,
MaximumMarks=p_MaximumMarks
WHERE AssignmentId=p_AssignmentId;

SELECT
a.AssignmentId,
a.Title,
a.AcademicYearId,
ay.AcademicYearName,
a.AcademicLevel,
a.GroupId,
g.GroupName,
a.SubjectId,
s.SubjectName,
a.FacultyId,
CONCAT(f.FirstName,' ',f.LastName) FacultyName,
a.Description,
a.DueDate,
a.Attachment,
a.MaximumMarks
FROM Assignments a
INNER JOIN AcademicYears ay
ON ay.AcademicYearId=a.AcademicYearId
INNER JOIN Groups g
ON g.GroupId=a.GroupId
INNER JOIN Subjects s
ON s.SubjectId=a.SubjectId
INNER JOIN Faculties f
ON f.Id=a.FacultyId
WHERE a.AssignmentId=p_AssignmentId;

END$$

DELIMITER ;