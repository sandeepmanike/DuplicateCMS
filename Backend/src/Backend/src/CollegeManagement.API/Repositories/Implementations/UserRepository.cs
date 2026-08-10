using CollegeManagement.API.Repositories.Interfaces;
using CollegeManagement.API.Data;
using CollegeManagement.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace CollegeManagement.API.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        private IDbConnection Connection => _context.Database.GetDbConnection();

        public async Task<User?> GetByEmailOrPhoneAsync(string emailOrPhone)
        {
            var user = await Connection.QueryFirstOrDefaultAsync<User>(
                "usp_GetUserByEmailOrPhone",
                new { p_EmailOrPhone = emailOrPhone },
                commandType: CommandType.StoredProcedure);
            if (user != null)
            {
                user.Role = await _context.Roles.FindAsync(user.RoleId) ?? null!;
            }
            return user;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var user = await Connection.QueryFirstOrDefaultAsync<User>(
                "usp_GetUserByEmail",
                new { p_Email = email },
                commandType: CommandType.StoredProcedure);
            if (user != null)
            {
                user.Role = await _context.Roles.FindAsync(user.RoleId) ?? null!;
            }
            return user;
        }

        public async Task AddAsync(User user)
        {
            var id = await Connection.ExecuteScalarAsync<int>(
                "usp_AddUser",
                new
                {
                    p_FullName = user.FullName,
                    p_Email = user.Email,
                    p_PhoneNumber = user.PhoneNumber,
                    p_PasswordHash = user.PasswordHash,
                    p_RoleId = user.RoleId
                },
                commandType: CommandType.StoredProcedure);
            user.UserId = id;
        }

        public async Task UpdateAsync(User user)
        {
            await Connection.ExecuteAsync(
                "usp_UpdateUser",
                new
                {
                    p_UserId = user.UserId,
                    p_FullName = user.FullName,
                    p_Email = user.Email,
                    p_PhoneNumber = user.PhoneNumber,
                    p_PasswordHash = user.PasswordHash,
                    p_RoleId = user.RoleId
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<Role?> GetRoleByNameAsync(string roleName)
        {
            return await Connection.QueryFirstOrDefaultAsync<Role>(
                "usp_GetRoleByName",
                new { p_RoleName = roleName },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = await Connection.QueryAsync<User>(
                "usp_GetAllUsers",
                commandType: CommandType.StoredProcedure);
            var userList = users.ToList();
            foreach (var user in userList)
            {
                user.Role = await _context.Roles.FindAsync(user.RoleId) ?? null!;
            }
            return userList;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            var user = await Connection.QueryFirstOrDefaultAsync<User>(
                "usp_GetUserById",
                new { p_UserId = id },
                commandType: CommandType.StoredProcedure);
            if (user != null)
            {
                user.Role = await _context.Roles.FindAsync(user.RoleId) ?? null!;
            }
            return user;
        }
    }
}
