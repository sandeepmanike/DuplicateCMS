DROP PROCEDURE IF EXISTS sp_GetRoomById;
DELIMITER //
CREATE PROCEDURE sp_GetRoomById(IN p_RoomId INT)
BEGIN
    SELECT 
        RoomId,
        COALESCE(RoomCode, RoomNumber, '') AS RoomCode,
        COALESCE(RoomName, RoomNumber, '') AS RoomName,
        RoomNumber,
        BuildingName AS Building,
        BuildingName,
        Floor,
        Capacity,
        RoomType,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM Rooms
    WHERE RoomId = p_RoomId;
END //
DELIMITER ;
