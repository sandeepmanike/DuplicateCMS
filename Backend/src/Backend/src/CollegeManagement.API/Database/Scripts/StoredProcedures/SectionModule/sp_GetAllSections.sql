DROP PROCEDURE IF EXISTS sp_GetAllSections;
DELIMITER //
CREATE PROCEDURE sp_GetAllSections()
BEGIN
    SELECT s.SectionId, s.Board, s.AcademicYearId, ay.AcademicYearName, s.Group, s.AcademicLevel,
           s.SectionName, s.RoomNumber, s.ClassTeacherId, 
           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS ClassTeacherName,
           s.MaximumStrength, s.IsActive, s.CreatedAt, s.UpdatedAt
    FROM Sections s
    LEFT JOIN AcademicYears ay ON ay.AcademicYearId = s.AcademicYearId
    LEFT JOIN Faculties f ON f.Id = s.ClassTeacherId
    ORDER BY s.SectionId DESC;
END //
DELIMITER ;
