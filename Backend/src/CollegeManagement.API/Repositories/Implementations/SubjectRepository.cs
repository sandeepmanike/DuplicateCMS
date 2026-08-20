using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using CollegeManagement.API.Data;
using CollegeManagement.API.Models;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly AppDbContext _context;

        public SubjectRepository(AppDbContext context) => _context = context;

        private async Task<IDbConnection> GetConnectionAsync()
        {
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();
            return conn;
        }

        private static string Clean(string value) => string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();

        public async Task<IEnumerable<Subject>> GetAllAsync() =>
            await (await GetConnectionAsync()).QueryAsync<Subject>("sp_GetAllSubjects", commandType: CommandType.StoredProcedure);

        public async Task<Subject?> GetByIdAsync(int subjectId) =>
            await (await GetConnectionAsync()).QueryFirstOrDefaultAsync<Subject>("sp_GetSubjectById", new { p_SubjectId = subjectId }, commandType: CommandType.StoredProcedure);

        public async Task<Subject> CreateAsync(Subject subject)
        {
            var c = await GetConnectionAsync();
            var result = await c.QueryFirstOrDefaultAsync<Subject>("sp_CreateSubject", new
            {
                p_BoardId = subject.BoardId,
                p_GroupId = subject.GroupId,
                p_AcademicLevelId = subject.AcademicLevelId,
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
                p_GroupId = subject.GroupId,
                p_AcademicLevelId = subject.AcademicLevelId,
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

        public async Task<bool> DeleteAsync(int subjectId) =>
            await (await GetConnectionAsync()).ExecuteScalarAsync<int>("sp_DeleteSubject", new { p_SubjectId = subjectId }, commandType: CommandType.StoredProcedure) > 0;

        public async Task<IEnumerable<Subject>> GetByGroupIdAsync(int groupId) =>
            await (await GetConnectionAsync()).QueryAsync<Subject>("sp_GetSubjectsByGroup", new { p_GroupId = groupId }, commandType: CommandType.StoredProcedure);

        public async Task<IEnumerable<Subject>> GetByContextAsync(int boardId, int groupId, int academicLevelId) =>
            await (await GetConnectionAsync()).QueryAsync<Subject>("sp_GetSubjectsByContext", new { p_BoardId = boardId, p_GroupId = groupId, p_AcademicLevelId = academicLevelId }, commandType: CommandType.StoredProcedure);

        public async Task<IEnumerable<Subject>> SearchAsync(string? search, int? boardId, int? groupId, int? academicLevelId, bool? isActive) =>
            await (await GetConnectionAsync()).QueryAsync<Subject>("sp_SearchSubjects", new
            {
                p_Search = string.IsNullOrWhiteSpace(search) ? null : search.Trim(),
                p_BoardId = boardId,
                p_GroupId = groupId,
                p_AcademicLevelId = academicLevelId,
                p_IsActive = isActive
            }, commandType: CommandType.StoredProcedure);

        public async Task<IEnumerable<Subject>> GetActiveAsync() =>
            await (await GetConnectionAsync()).QueryAsync<Subject>("sp_GetActiveSubjects", commandType: CommandType.StoredProcedure);

        public async Task<IEnumerable<Subject>> GetByBoardIdAsync(int boardId) =>
            await (await GetConnectionAsync()).QueryAsync<Subject>("sp_GetSubjectsByBoardId", new { p_BoardId = boardId }, commandType: CommandType.StoredProcedure);

        public async Task<bool> SubjectCodeExistsAsync(string subjectCode, int boardId, int groupId, int academicLevelId, int? excludeSubjectId = null) =>
            await (await GetConnectionAsync()).ExecuteScalarAsync<int>("sp_CheckSubjectCode", new
            {
                p_SubjectCode = subjectCode.Trim(),
                p_BoardId = boardId,
                p_GroupId = groupId,
                p_AcademicLevelId = academicLevelId,
                p_ExcludeSubjectId = excludeSubjectId
            }, commandType: CommandType.StoredProcedure) > 0;
    }
}
