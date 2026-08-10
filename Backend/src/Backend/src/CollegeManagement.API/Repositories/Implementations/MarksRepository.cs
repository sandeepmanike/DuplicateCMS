using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CollegeManagement.API.Data;
using CollegeManagement.API.Models;
using CollegeManagement.API.Repositories.Interfaces;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Repositories.Implementations
{
    public class MarksRepository : IMarksRepository
    {
        private readonly AppDbContext _context;

        public MarksRepository(AppDbContext context)
        {
            _context = context;
        }

        private IDbConnection Connection => _context.Database.GetDbConnection();

        public async Task<List<Mark>> GetAllAsync()
        {
            var result = await Connection.QueryAsync<Mark>("sp_GetAllMarks", commandType: CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<Mark?> GetByIdAsync(int markId)
        {
            return await Connection.QueryFirstOrDefaultAsync<Mark>("sp_GetMarkById", new { p_MarkId = markId }, commandType: CommandType.StoredProcedure);
        }

        public async Task<Mark?> CreateAsync(Mark mark)
        {
            return await Connection.QueryFirstOrDefaultAsync<Mark>(
                "sp_AddMark",
                new
                {
                    p_Board = mark.Board,
                    p_AcademicYearId = mark.AcademicYearId,
                    p_AcademicLevel = mark.AcademicLevel,
                    p_GroupId = mark.GroupId,
                    p_SectionId = mark.SectionId,
                    p_ExaminationId = mark.ExaminationId,
                    p_SubjectId = mark.SubjectId,
                    p_StudentId = mark.StudentId,
                    p_RollNo = mark.RollNo,
                    p_StudentName = mark.StudentName,
                    p_InternalMarks = mark.InternalMarks,
                    p_PracticalMarks = mark.PracticalMarks,
                    p_TheoryMarks = mark.TheoryMarks,
                    p_PassingMarks = mark.PassingMarks
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<Mark?> UpdateAsync(int markId, Mark mark)
        {
            return await Connection.QueryFirstOrDefaultAsync<Mark>(
                "sp_UpdateMark",
                new
                {
                    p_MarkId = markId,
                    p_InternalMarks = mark.InternalMarks,
                    p_PracticalMarks = mark.PracticalMarks,
                    p_TheoryMarks = mark.TheoryMarks,
                    p_PassingMarks = mark.PassingMarks
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> DeleteAsync(int markId)
        {
            var affected = await Connection.ExecuteScalarAsync<int>("sp_DeleteMark", new { p_MarkId = markId }, commandType: CommandType.StoredProcedure);
            return affected > 0;
        }

        public async Task<bool> RestoreAsync(int markId)
        {
            var affected = await Connection.ExecuteAsync("sp_RestoreMark", new { p_MarkId = markId }, commandType: CommandType.StoredProcedure);
            return affected > 0;
        }

        public async Task<List<Mark>> GetByStudentAsync(int studentId)
        {
            var result = await Connection.QueryAsync<Mark>("sp_GetMarksByStudent", new { p_StudentId = studentId }, commandType: CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<List<Mark>> GetBySubjectAsync(int subjectId)
        {
            var result = await Connection.QueryAsync<Mark>("sp_GetMarksBySubject", new { p_SubjectId = subjectId }, commandType: CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<List<Mark>> GetByExamAsync(int examinationId)
        {
            var result = await Connection.QueryAsync<Mark>("sp_GetMarksByExam", new { p_ExaminationId = examinationId }, commandType: CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<int> VerifyMarksAsync(int examinationId, int? subjectId, int? sectionId, string verifiedBy)
        {
            return await Connection.ExecuteScalarAsync<int>("sp_VerifyMarks", new { p_ExaminationId = examinationId, p_SubjectId = subjectId, p_SectionId = sectionId, p_VerifiedBy = verifiedBy }, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> PublishMarksAsync(int examinationId, int? subjectId, int? sectionId)
        {
            return await Connection.ExecuteScalarAsync<int>("sp_PublishMarks", new { p_ExaminationId = examinationId, p_SubjectId = subjectId, p_SectionId = sectionId }, commandType: CommandType.StoredProcedure);
        }
    }
}