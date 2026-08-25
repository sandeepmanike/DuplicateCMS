DROP PROCEDURE IF EXISTS sp_GetAllSections;
DELIMITER //
CREATE PROCEDURE sp_GetAllSections(
    IN p_Board VARCHAR(100),
    IN p_AcademicYearId INT,
    IN p_Group VARCHAR(100),
    IN p_GroupId INT,
    IN p_Programme VARCHAR(100),
    IN p_AcademicLevel VARCHAR(50),
    IN p_SearchTerm VARCHAR(100),
    IN p_IsActive TINYINT(1)
)
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
           COALESCE(r.BlockName, '') AS BlockName,
           COALESCE(r.BlockName, '') AS BuildingName,
           COALESCE(r.BlockName, '') AS Building,
           COALESCE(r.BlockName, '') AS Block,
           COALESCE(s.InchargeId, s.ClassTeacherId) AS InchargeId,
           COALESCE(s.InchargeId, s.ClassTeacherId) AS ClassTeacherId,
           COALESCE(s.InchargeId, s.ClassTeacherId) AS TeacherId,
           COALESCE(s.InchargeId, s.ClassTeacherId) AS FacultyId,
           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS InchargeName,
           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS Incharge,
           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS ClassTeacherName,
           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS Teacher,
           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS FacultyName,
           COALESCE(f.EmployeeId, '') AS FacultyEmployeeId,
           s.MaximumStrength,
           s.IsActive,
           s.CreatedAt,
           s.UpdatedAt
    FROM Sections s
    LEFT JOIN AcademicYears ay ON ay.AcademicYearId = s.AcademicYearId
    LEFT JOIN Faculties f ON f.Id = COALESCE(s.InchargeId, s.ClassTeacherId)
    LEFT JOIN Rooms r ON r.RoomId = s.RoomId
    WHERE (p_Board IS NULL OR p_Board = '' OR s.Board = p_Board)
      AND (p_AcademicYearId IS NULL OR p_AcademicYearId = 0 OR s.AcademicYearId = p_AcademicYearId)
      AND (p_Group IS NULL OR p_Group = '' OR s.`Group` = p_Group)
      AND (p_GroupId IS NULL OR p_GroupId = 0 OR s.GroupId = p_GroupId)
      AND (p_Programme IS NULL OR p_Programme = '' OR s.Programme = p_Programme)
      AND (p_AcademicLevel IS NULL OR p_AcademicLevel = '' OR s.AcademicLevel = p_AcademicLevel)
      AND (p_IsActive IS NULL OR s.IsActive = p_IsActive)
      AND (p_SearchTerm IS NULL OR p_SearchTerm = '' OR (
           s.SectionName LIKE CONCAT('%', p_SearchTerm, '%') OR
           s.`Group` LIKE CONCAT('%', p_SearchTerm, '%') OR
           s.Programme LIKE CONCAT('%', p_SearchTerm, '%') OR
           CONCAT(f.FirstName, ' ', f.LastName) LIKE CONCAT('%', p_SearchTerm, '%') OR
           s.RoomNumber LIKE CONCAT('%', p_SearchTerm, '%') OR
           r.RoomName LIKE CONCAT('%', p_SearchTerm, '%') OR
           r.RoomNumber LIKE CONCAT('%', p_SearchTerm, '%')
      ))
    ORDER BY s.SectionId DESC;
END //
DELIMITER ;
