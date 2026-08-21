using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CollegeManagement.API.Data;
using CollegeManagement.API.Models.Timetable;
using CollegeManagement.API.Repositories.Interfaces;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Repositories.Implementations
{
    public class RoomRepository : IRoomRepository
    {
        private readonly AppDbContext _context;

        public RoomRepository(AppDbContext context)
        {
            _context = context;
        }

        private IDbConnection Connection => _context.Database.GetDbConnection();

        public async Task<IEnumerable<Room>> GetAllAsync()
        {
            return await Connection.QueryAsync<Room>(
                "sp_GetRooms",
                commandType: CommandType.StoredProcedure);
        }

        public async Task<Room?> GetByIdAsync(int id)
        {
            return await Connection.QueryFirstOrDefaultAsync<Room>(
                "sp_GetRoomById",
                new { p_RoomId = id },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<Room?> GetByCodeAsync(string roomCode)
        {
            var rooms = await Connection.QueryAsync<Room>(
                "sp_GetRooms",
                commandType: CommandType.StoredProcedure);

            return rooms.FirstOrDefault(r => (r.RoomCode ?? r.RoomNumber).Equals(roomCode, System.StringComparison.OrdinalIgnoreCase));
        }

        public async Task<Room> AddAsync(Room room)
        {
            var id = await Connection.ExecuteScalarAsync<int>(
                "sp_CreateRoom",
                new
                {
                    p_RoomCode = room.RoomCode ?? room.RoomNumber,
                    p_RoomName = room.RoomName ?? room.RoomCode ?? room.RoomNumber,
                    p_Capacity = room.Capacity,
                    p_RoomType = room.RoomType,
                    p_Building = room.Building ?? room.BuildingName,
                    p_Floor = room.Floor,
                    p_IsActive = room.IsActive
                },
                commandType: CommandType.StoredProcedure);

            room.RoomId = id;
            return room;
        }

        public async Task UpdateAsync(Room room)
        {
            await Connection.ExecuteAsync(
                "sp_UpdateRoom",
                new
                {
                    p_RoomId = room.RoomId,
                    p_RoomCode = room.RoomCode ?? room.RoomNumber,
                    p_RoomName = room.RoomName ?? room.RoomCode ?? room.RoomNumber,
                    p_Capacity = room.Capacity,
                    p_RoomType = room.RoomType,
                    p_Building = room.Building ?? room.BuildingName,
                    p_Floor = room.Floor,
                    p_IsActive = room.IsActive
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int id)
        {
            await Connection.ExecuteAsync(
                "sp_DeleteRoom",
                new { p_RoomId = id },
                commandType: CommandType.StoredProcedure);
        }
    }
}
