DROP PROCEDURE IF EXISTS sp_DeleteBoard;

CREATE PROCEDURE sp_DeleteBoard(
    IN p_BoardId INT
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;

    UPDATE Boards
    SET IsActive = 0,
        UpdatedAt = UTC_TIMESTAMP()
    WHERE BoardId = p_BoardId;

    COMMIT;
    
    SELECT ROW_COUNT() AS AffectedRows;
END;
