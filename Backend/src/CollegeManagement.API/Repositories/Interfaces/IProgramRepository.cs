using CollegeManagement.API.DTOs.Program;

namespace CollegeManagement.API.Repositories
{
    public interface IProgramRepository
    {
        Task<IEnumerable<ProgramDto>> GetAllAsync();

        Task<ProgramDto?> GetByIdAsync(int programId);

        Task<ProgramDto?> CreateAsync(CreateProgramDto dto);

        Task<ProgramDto?> UpdateAsync(
            int programId,
            UpdateProgramDto dto);

        Task<bool> SetStatusAsync(
            int programId,
            bool isActive);

        Task<IEnumerable<ProgramDto>> GetByGroupIdAsync(
            int groupId);
    }
}
namespace CollegeManagement.API.Repositories.Interfaces
{
    public interface IProgramRepository
    {
    }
}
