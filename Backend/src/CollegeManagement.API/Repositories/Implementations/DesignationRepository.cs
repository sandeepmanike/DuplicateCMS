using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CollegeManagement.API.Data;
using CollegeManagement.API.Models.Faculty;
using CollegeManagement.API.Repositories.Interfaces;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Repositories.Implementations
{
    public class DesignationRepository : IDesignationRepository
    {
        private readonly AppDbContext _context;

        public DesignationRepository(AppDbContext context)
        {
            _context = context;
        }

        private bool IsRelational => _context.Database.ProviderName != null && !_context.Database.ProviderName.Contains("InMemory");

        private IDbConnection Connection => _context.Database.GetDbConnection();

        public async Task<IEnumerable<Designation>> GetAllAsync(bool includeInactive = false)
        {
            if (IsRelational)
            {
                return await Connection.QueryAsync<Designation>(
                    "sp_GetDesignations",
                    new { p_IncludeInactive = includeInactive ? 1 : 0 },
                    commandType: CommandType.StoredProcedure);
            }
            else
            {
                return await _context.Designations
                    .Where(d => includeInactive || d.IsActive)
                    .OrderBy(d => d.Name)
                    .ToListAsync();
            }
        }

        public async Task<Designation?> GetByIdAsync(int id)
        {
            if (IsRelational)
            {
                return await Connection.QueryFirstOrDefaultAsync<Designation>(
                    "sp_GetDesignationById",
                    new { p_Id = id },
                    commandType: CommandType.StoredProcedure);
            }
            else
            {
                return await _context.Designations.FirstOrDefaultAsync(d => d.Id == id);
            }
        }

        public async Task<Designation?> GetByNameAsync(string name)
        {
            if (IsRelational)
            {
                return await Connection.QueryFirstOrDefaultAsync<Designation>(
                    "sp_GetDesignationByName",
                    new { p_Name = name.Trim() },
                    commandType: CommandType.StoredProcedure);
            }
            else
            {
                var trimmed = name.Trim().ToLower();
                return await _context.Designations.FirstOrDefaultAsync(d => d.Name.ToLower() == trimmed);
            }
        }

        public async Task<bool> IsNameUniqueAsync(string name, int? excludeId = null)
        {
            if (IsRelational)
            {
                int count = await Connection.ExecuteScalarAsync<int>(
                    "sp_CheckDesignationNameUnique",
                    new { p_Name = name.Trim(), p_ExcludeId = excludeId },
                    commandType: CommandType.StoredProcedure);
                return count == 0;
            }
            else
            {
                var trimmed = name.Trim().ToLower();
                return !await _context.Designations.AnyAsync(d => d.Name.ToLower() == trimmed && (excludeId == null || d.Id != excludeId));
            }
        }

        public async Task<bool> IsAssignedToFacultyAsync(int designationId)
        {
            if (IsRelational)
            {
                int count = await Connection.ExecuteScalarAsync<int>(
                    "sp_CheckDesignationAssignedToFaculty",
                    new { p_DesignationId = designationId },
                    commandType: CommandType.StoredProcedure);
                return count > 0;
            }
            else
            {
                return await _context.Faculties.AnyAsync(f => f.DesignationId == designationId && !f.IsDeleted);
            }
        }

        public async Task<Designation> AddAsync(Designation designation)
        {
            if (IsRelational)
            {
                int id = await Connection.ExecuteScalarAsync<int>(
                    "sp_CreateDesignation",
                    new
                    {
                        p_Name = designation.Name.Trim(),
                        p_IsActive = designation.IsActive ? 1 : 0
                    },
                    commandType: CommandType.StoredProcedure);

                designation.Id = id;
                return designation;
            }
            else
            {
                await _context.Designations.AddAsync(designation);
                await _context.SaveChangesAsync();
                return designation;
            }
        }

        public async Task UpdateAsync(Designation designation)
        {
            if (IsRelational)
            {
                await Connection.ExecuteAsync(
                    "sp_UpdateDesignation",
                    new
                    {
                        p_Id = designation.Id,
                        p_Name = designation.Name.Trim(),
                        p_IsActive = designation.IsActive ? 1 : 0
                    },
                    commandType: CommandType.StoredProcedure);
            }
            else
            {
                var existing = await _context.Designations.FirstOrDefaultAsync(d => d.Id == designation.Id);
                if (existing != null)
                {
                    existing.Name = designation.Name.Trim();
                    existing.IsActive = designation.IsActive;
                    existing.UpdatedAt = System.DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            if (IsRelational)
            {
                await Connection.ExecuteAsync(
                    "sp_DeleteDesignation",
                    new { p_Id = id },
                    commandType: CommandType.StoredProcedure);
            }
            else
            {
                var existing = await _context.Designations.FirstOrDefaultAsync(d => d.Id == id);
                if (existing != null)
                {
                    _context.Designations.Remove(existing);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
