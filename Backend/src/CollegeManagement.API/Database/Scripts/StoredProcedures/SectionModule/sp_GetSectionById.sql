DROP PROCEDURE IF EXISTS sp_GetSectionById;
DELIMITER //
CREATE PROCEDURE sp_GetSectionById(IN p_SectionId INT)
BEGIN
    SELECT s.SectionId,
           s.BoardId,
           s.Board,
           s.AcademicYearId,
           COALESCE(ay.AcademicYearName, '') AS AcademicYearName,
           s.GroupId,
           s.`Group`,
           COALESCE(s.Programme, '') AS Programme,
           s.AcademicLevel,
           s.SectionName,
           s.RoomNumber,
           s.RoomId,
           COALESCE(r.RoomName, r.RoomNumber, s.RoomNumber, '') AS RoomName,
           s.InchargeId,
           s.InchargeId AS ClassTeacherId,
           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS InchargeName,
           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS Incharge,
           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS ClassTeacherName,
           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS Teacher,
           s.MaximumStrength,
           s.IsActive,
           s.CreatedAt,
           s.UpdatedAt
    FROM Sections s
    LEFT JOIN AcademicYears ay ON ay.AcademicYearId = s.AcademicYearId
    LEFT JOIN Faculties f ON f.Id = s.InchargeId
    LEFT JOIN Rooms r ON r.RoomId = s.RoomId
    WHERE s.SectionId = p_SectionId;
END //
DELIMITER ;
