using CollegeManagement.API.Data;
using CollegeManagement.API.Models;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CollegeManagement.API.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly AppDbContext _context;
        public SubjectRepository(AppDbContext context) => _context = context;
        private async Task<System.Data.Common.DbConnection> GetConnectionAsync()
        {
            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open) await connection.OpenAsync();
            return connection;
        }
        private static string Clean(string? value) => value?.Trim() ?? string.Empty;

        public async Task<IEnumerable<Subject>> GetAllAsync() => await (await GetConnectionAsync()).QueryAsync<Subject>("sp_GetAllSubjects", commandType: CommandType.StoredProcedure);
        public async Task<Subject?> GetByIdAsync(int subjectId) => await (await GetConnectionAsync()).QueryFirstOrDefaultAsync<Subject>("sp_GetSubjectById", new { p_SubjectId = subjectId }, commandType: CommandType.StoredProcedure);

        public async Task<Subject> CreateAsync(Subject subject)
        {
            var c = await GetConnectionAsync();
            var result = await c.QueryFirstOrDefaultAsync<Subject>("sp_CreateSubject", new
            {
                p_BoardId = subject.BoardId,
                p_AcademicYearId = subject.AcademicYearId,
                p_AcademicLevelId = subject.AcademicLevelId,
                p_GroupId = subject.GroupId,
                p_SubjectName = Clean(subject.SubjectName),
                p_SubjectCode = Clean(subject.SubjectCode),
                p_SubjectType = Clean(subject.SubjectType),
                p_Theory = subject.Theory,
                p_Practical = subject.Practical,
                p_Language = subject.Language,
                p_Elective = subject.Elective,
                p_InternalMarks = subject.InternalMarks,
                p_PracticalMarks = subject.PracticalMarks,
                p_ExternalMarks = subject.ExternalMarks,
                p_TotalMarks = subject.TotalMarks,
                p_PassingMarks = subject.PassingMarks,
                p_IsActive = subject.IsActive
            }, commandType: CommandType.StoredProcedure);
            return result ?? throw new InvalidOperationException("Subject was created, but no response was returned.");
        }

        public async Task<Subject?> UpdateAsync(int subjectId, Subject subject)
        {
            var c = await GetConnectionAsync();
            return await c.QueryFirstOrDefaultAsync<Subject>("sp_UpdateSubject", new
            {
                p_SubjectId = subjectId,
                p_BoardId = subject.BoardId,
                p_AcademicYearId = subject.AcademicYearId,
                p_AcademicLevelId = subject.AcademicLevelId,
                p_GroupId = subject.GroupId,
                p_SubjectName = Clean(subject.SubjectName),
                p_SubjectCode = Clean(subject.SubjectCode),
                p_SubjectType = Clean(subject.SubjectType),
                p_Theory = subject.Theory,
                p_Practical = subject.Practical,
                p_Language = subject.Language,
                p_Elective = subject.Elective,
                p_InternalMarks = subject.InternalMarks,
                p_PracticalMarks = subject.PracticalMarks,
                p_ExternalMarks = subject.ExternalMarks,
                p_TotalMarks = subject.TotalMarks,
                p_PassingMarks = subject.PassingMarks,
                p_IsActive = subject.IsActive
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> DeleteAsync(int subjectId) => await (await GetConnectionAsync()).ExecuteScalarAsync<int>("sp_DeleteSubject", new { p_SubjectId = subjectId }, commandType: CommandType.StoredProcedure) > 0;
        public async Task<IEnumerable<Subject>> GetByGroupIdAsync(int groupId) => await (await GetConnectionAsync()).QueryAsync<Subject>("sp_GetSubjectsByGroup", new { p_GroupId = groupId }, commandType: CommandType.StoredProcedure);
        public async Task<IEnumerable<Subject>> SearchAsync(string? search, int? boardId, int? academicYearId, int? groupId, bool? isActive) => await (await GetConnectionAsync()).QueryAsync<Subject>("sp_SearchSubjects", new { p_Search = string.IsNullOrWhiteSpace(search) ? null : search.Trim(), p_BoardId = boardId, p_AcademicYearId = academicYearId, p_GroupId = groupId, p_IsActive = isActive }, commandType: CommandType.StoredProcedure);
        public async Task<IEnumerable<Subject>> GetActiveAsync() => await (await GetConnectionAsync()).QueryAsync<Subject>("sp_GetActiveSubjects", commandType: CommandType.StoredProcedure);
        public async Task<IEnumerable<Subject>> GetByBoardIdAsync(int boardId) => await (await GetConnectionAsync()).QueryAsync<Subject>("sp_GetSubjectsByBoardId", new { p_BoardId = boardId }, commandType: CommandType.StoredProcedure);
        public async Task<IEnumerable<Subject>> GetByAcademicYearIdAsync(int academicYearId) => await (await GetConnectionAsync()).QueryAsync<Subject>("sp_GetSubjectsByAcademicYear", new { p_AcademicYearId = academicYearId }, commandType: CommandType.StoredProcedure);
        public async Task<bool> SubjectCodeExistsAsync(string subjectCode, int? excludeSubjectId = null) => await (await GetConnectionAsync()).ExecuteScalarAsync<int>("sp_CheckSubjectCode", new { p_SubjectCode = subjectCode.Trim(), p_ExcludeSubjectId = excludeSubjectId }, commandType: CommandType.StoredProcedure) > 0;
    }
}
