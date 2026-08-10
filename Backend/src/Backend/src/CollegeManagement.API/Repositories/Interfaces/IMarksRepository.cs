using System.Collections.Generic;
using System.Threading.Tasks;
using CollegeManagement.API.Models;

namespace CollegeManagement.API.Repositories.Interfaces
{
    public interface IMarksRepository
    {
        Task<List<Mark>> GetAllAsync();
        Task<Mark?> GetByIdAsync(int markId);
        Task<Mark?> CreateAsync(Mark mark);
        Task<Mark?> UpdateAsync(int markId, Mark mark);
        Task<bool> DeleteAsync(int markId);
        Task<bool> RestoreAsync(int markId);
        Task<List<Mark>> GetByStudentAsync(int studentId);
        Task<List<Mark>> GetBySubjectAsync(int subjectId);
        Task<List<Mark>> GetByExamAsync(int examinationId);
        Task<int> VerifyMarksAsync(int examinationId, int? subjectId, int? sectionId, string verifiedBy);
        Task<int> PublishMarksAsync(int examinationId, int? subjectId, int? sectionId);
    }
}