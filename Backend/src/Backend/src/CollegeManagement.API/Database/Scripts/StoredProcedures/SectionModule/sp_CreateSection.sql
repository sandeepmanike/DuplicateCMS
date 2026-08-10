DROP PROCEDURE IF EXISTS sp_CreateSection;
DELIMITER //
CREATE PROCEDURE sp_CreateSection(
    IN p_Board VARCHAR(100),
    IN p_AcademicYearId INT,
    IN p_Group VARCHAR(100),
    IN p_AcademicLevel VARCHAR(50),
    IN p_SectionName VARCHAR(50),
    IN p_RoomNumber VARCHAR(50),
    IN p_ClassTeacherId INT,
    IN p_MaximumStrength INT,
    IN p_IsActive TINYINT(1)
)
BEGIN
    INSERT INTO Sections (Board, AcademicYearId, `Group`, AcademicLevel, SectionName, RoomNumber, ClassTeacherId, MaximumStrength, IsActive, CreatedAt)
    VALUES (p_Board, p_AcademicYearId, p_Group, p_AcademicLevel, p_SectionName, p_RoomNumber, p_ClassTeacherId, p_MaximumStrength, p_IsActive, UTC_TIMESTAMP());

    SELECT s.SectionId, s.Board, s.AcademicYearId, ay.AcademicYearName, s.Group, s.AcademicLevel,
           s.SectionName, s.RoomNumber, s.ClassTeacherId,
           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS ClassTeacherName,
           s.MaximumStrength, s.IsActive, s.CreatedAt, s.UpdatedAt
    FROM Sections s
    LEFT JOIN AcademicYears ay ON ay.AcademicYearId = s.AcademicYearId
    LEFT JOIN Faculties f ON f.Id = s.ClassTeacherId
    WHERE s.SectionId = LAST_INSERT_ID();
END //
DELIMITER ;
