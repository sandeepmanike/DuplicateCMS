using CollegeManagement.API.Repositories.Interfaces;
using CollegeManagement.API.Data;
using CollegeManagement.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using System.Linq;

namespace CollegeManagement.API.Repositories.Implementations
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;

        public AdminRepository(AppDbContext context)
        {
            _context = context;
        }

        private IDbConnection Connection => _context.Database.GetDbConnection();

        public async Task<IEnumerable<Admin>> GetAllAsync()
        {
            return await Connection.QueryAsync<Admin>("sp_GetAllAdmins", commandType: CommandType.StoredProcedure);
        }

        public async Task<Admin?> GetByIdAsync(int id)
        {
            return await Connection.QueryFirstOrDefaultAsync<Admin>("sp_GetAdminById", new { p_Id = id }, commandType: CommandType.StoredProcedure);
        }

        public async Task<Admin?> GetByEmailAsync(string email)
        {
            return await Connection.QueryFirstOrDefaultAsync<Admin>("sp_GetAdminByEmail", new { p_Email = email }, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> AddAsync(Admin admin)
        {
            var id = await Connection.ExecuteScalarAsync<int>("sp_CreateAdmin", 
                new { p_Email = admin.Email, p_Password = admin.Password, p_IsActive = admin.IsActive }, 
                commandType: CommandType.StoredProcedure);
            admin.Id = id;
            return id;
        }

        public async Task UpdateStatusAsync(int id, bool isActive)
        {
            await Connection.ExecuteAsync("sp_UpdateAdminStatus", 
                new { p_Id = id, p_IsActive = isActive }, 
                commandType: CommandType.StoredProcedure);
        }

        public async Task UpdatePasswordAsync(int id, string newPasswordHash)
        {
            await Connection.ExecuteAsync("sp_ChangeAdminPassword", 
                new { p_Id = id, p_Password = newPasswordHash }, 
                commandType: CommandType.StoredProcedure);
        }
    }
}
