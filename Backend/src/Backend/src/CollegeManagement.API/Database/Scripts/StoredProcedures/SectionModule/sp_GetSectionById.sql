DROP PROCEDURE IF EXISTS sp_GetSectionById;
DELIMITER //
CREATE PROCEDURE sp_GetSectionById(IN p_SectionId INT)
BEGIN
    SELECT s.SectionId, s.Board, s.AcademicYearId, ay.AcademicYearName, s.Group, s.AcademicLevel,
           s.SectionName, s.RoomNumber, s.ClassTeacherId, 
           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS ClassTeacherName,
           s.MaximumStrength, s.IsActive, s.CreatedAt, s.UpdatedAt
    FROM Sections s
    LEFT JOIN AcademicYears ay ON ay.AcademicYearId = s.AcademicYearId
    LEFT JOIN Faculties f ON f.Id = s.ClassTeacherId
    WHERE s.SectionId = p_SectionId;
END //
DELIMITER ;
