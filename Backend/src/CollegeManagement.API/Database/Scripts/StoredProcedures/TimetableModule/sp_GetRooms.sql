DROP PROCEDURE IF EXISTS sp_GetRooms;
DELIMITER //
CREATE PROCEDURE sp_GetRooms()
BEGIN
    SELECT 
        RoomId,
        RoomNumber,
        BuildingName,
        Floor,
        Capacity,
        RoomType,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM Rooms
    ORDER BY BuildingName ASC, RoomNumber ASC;
END //
DELIMITER ;
