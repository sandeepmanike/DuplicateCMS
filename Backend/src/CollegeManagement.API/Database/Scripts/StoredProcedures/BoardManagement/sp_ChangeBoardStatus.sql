DROP PROCEDURE IF EXISTS sp_ChangeBoardStatus;

CREATE PROCEDURE sp_ChangeBoardStatus(
    IN p_BoardId INT,
    IN p_Status BOOLEAN
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;

    UPDATE Boards
    SET IsActive = p_Status,
        UpdatedAt = UTC_TIMESTAMP()
    WHERE BoardId = p_BoardId;

    COMMIT;
    
    SELECT ROW_COUNT() AS AffectedRows;
END;
