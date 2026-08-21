DROP PROCEDURE IF EXISTS sp_GetSectionsByGroup;
DELIMITER //
CREATE PROCEDURE sp_GetSectionsByGroup(IN p_GroupId INT)
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
           s.ClassTeacherId,
           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS ClassTeacherName,
           s.MaximumStrength,
           s.IsActive,
           s.CreatedAt,
           s.UpdatedAt
    FROM Sections s
    LEFT JOIN AcademicYears ay ON ay.AcademicYearId = s.AcademicYearId
    LEFT JOIN Faculties f ON f.Id = s.ClassTeacherId
    LEFT JOIN Rooms r ON r.RoomId = s.RoomId
    WHERE s.GroupId = p_GroupId 
       OR s.`Group` = (SELECT GroupName FROM `Groups` WHERE GroupId = p_GroupId LIMIT 1)
    ORDER BY s.SectionName ASC;
END //
DELIMITER ;
