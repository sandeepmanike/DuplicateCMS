DROP PROCEDURE IF EXISTS sp_GetBoards;

CREATE PROCEDURE sp_GetBoards(
    IN p_BoardName VARCHAR(100),
    IN p_BoardCode VARCHAR(30),
    IN p_CountryId INT,
    IN p_StateId INT,
    IN p_Status BOOLEAN
)
BEGIN
    SELECT 
        b.BoardId, b.BoardCode, b.BoardName, b.Description, b.InternalAssessment, b.PracticalExams, b.BoardExams, b.PassPercentage, b.RankCalculation, b.IsActive, b.CreatedAt, b.UpdatedAt,
        c.CountryId, c.CountryCode, c.CountryName,
        s.StateId, s.StateCode, s.StateName,
        ap.AcademicPatternId, ap.PatternCode, ap.PatternName,
        gs.GradingSystemId, gs.GradingSystemCode, gs.GradingSystemName
    FROM Boards b
    INNER JOIN Countries c ON b.CountryId = c.CountryId
    LEFT JOIN States s ON b.StateId = s.StateId
    INNER JOIN AcademicPatterns ap ON b.AcademicPatternId = ap.AcademicPatternId
    INNER JOIN GradingSystems gs ON b.GradingSystemId = gs.GradingSystemId
    WHERE (p_BoardName IS NULL OR TRIM(p_BoardName) = '' OR b.BoardName LIKE CONCAT('%', TRIM(p_BoardName), '%'))
      AND (p_BoardCode IS NULL OR TRIM(p_BoardCode) = '' OR b.BoardCode LIKE CONCAT('%', TRIM(p_BoardCode), '%'))
      AND (p_CountryId IS NULL OR b.CountryId = p_CountryId)
      AND (p_StateId IS NULL OR b.StateId = p_StateId)
      AND (p_Status IS NULL OR b.IsActive = p_Status)
    ORDER BY b.BoardName ASC;
END;
