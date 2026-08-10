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
    public class AcademicYearRepository : IAcademicYearRepository
    {
        static AcademicYearRepository()
        {
            SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
        }

        private readonly AppDbContext _context;
        public AcademicYearRepository(AppDbContext context)
        {
            _context = context;
        }

        private IDbConnection Connection => _context.Database.GetDbConnection();

        public async Task<IEnumerable<AcademicYear>> GetAllAsync()
        {
            return await Connection.QueryAsync<AcademicYear>(
                "usp_GetAllAcademicYears",
                commandType: CommandType.StoredProcedure);
        }

        public async Task<AcademicYear?> GetByIdAsync(int id)
        {
            return await Connection.QueryFirstOrDefaultAsync<AcademicYear>(
                "usp_GetAcademicYearById",
                new { p_Id = id },
                commandType: CommandType.StoredProcedure);
        }

        public async Task AddAsync(AcademicYear academicYear)
        {
            var id = await Connection.ExecuteScalarAsync<int>(
                "usp_AddAcademicYear",
                new
                {
                    p_AcademicYearName = academicYear.AcademicYearName,
                    p_StartDate = academicYear.StartDate,
                    p_EndDate = academicYear.EndDate,
                    p_AdmissionStartDate = academicYear.AdmissionStartDate,
                    p_AdmissionEndDate = academicYear.AdmissionEndDate,
                    p_IsActive = academicYear.IsActive
                },
                commandType: CommandType.StoredProcedure);
            academicYear.AcademicYearId = id;
        }

        public async Task UpdateAsync(AcademicYear academicYear)
        {
            await Connection.ExecuteAsync(
                "usp_UpdateAcademicYear",
                new
                {
                    p_AcademicYearId = academicYear.AcademicYearId,
                    p_AcademicYearName = academicYear.AcademicYearName,
                    p_StartDate = academicYear.StartDate,
                    p_EndDate = academicYear.EndDate,
                    p_AdmissionStartDate = academicYear.AdmissionStartDate,
                    p_AdmissionEndDate = academicYear.AdmissionEndDate,
                    p_IsActive = academicYear.IsActive
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(AcademicYear academicYear)
        {
            await Connection.ExecuteAsync(
                "usp_DeleteAcademicYear",
                new { p_AcademicYearId = academicYear.AcademicYearId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task DeactivateAllExceptAsync(int activeId)
        {
            await Connection.ExecuteAsync(
                "usp_DeactivateAllExcept",
                new { p_ActiveId = activeId },
                commandType: CommandType.StoredProcedure);
        }
    }

    public class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
    {
        public override void SetValue(IDbDataParameter parameter, DateOnly value)
        {
            parameter.Value = value.ToDateTime(TimeOnly.MinValue);
        }

        public override DateOnly Parse(object value)
        {
            if (value is DateTime dateTime)
            {
                return DateOnly.FromDateTime(dateTime);
            }
            return DateOnly.FromDateTime(Convert.ToDateTime(value));
        }
    }
}
