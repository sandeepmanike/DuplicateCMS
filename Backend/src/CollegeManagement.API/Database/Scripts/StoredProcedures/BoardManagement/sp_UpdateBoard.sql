DROP PROCEDURE IF EXISTS sp_UpdateBoard;

CREATE PROCEDURE sp_UpdateBoard(
    IN p_BoardId INT,
    IN p_BoardName VARCHAR(100),
    IN p_BoardCode VARCHAR(30),
    IN p_Description VARCHAR(500),
    IN p_CountryId INT,
    IN p_StateId INT,
    IN p_AcademicPatternId INT,
    IN p_GradingSystemId INT,
    IN p_InternalAssessment BOOLEAN,
    IN p_PracticalExams BOOLEAN,
    IN p_BoardExams BOOLEAN,
    IN p_PassPercentage DECIMAL(5,2),
    IN p_RankCalculation BOOLEAN,
    IN p_IsActive BOOLEAN
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;

    UPDATE Boards
    SET
        BoardName = p_BoardName,
        BoardCode = TRIM(p_BoardCode),
        Description = p_Description,
        CountryId = p_CountryId,
        StateId = p_StateId,
        AcademicPatternId = p_AcademicPatternId,
        GradingSystemId = p_GradingSystemId,
        InternalAssessment = p_InternalAssessment,
        PracticalExams = p_PracticalExams,
        BoardExams = p_BoardExams,
        PassPercentage = p_PassPercentage,
        RankCalculation = p_RankCalculation,
        IsActive = p_IsActive,
        UpdatedAt = UTC_TIMESTAMP()
    WHERE BoardId = p_BoardId;

    COMMIT;
    
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
    WHERE b.BoardId = p_BoardId;
END;
