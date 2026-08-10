using System.Collections.Generic;
using System.Threading.Tasks;
using CollegeManagement.API.Models.Timetable;

namespace CollegeManagement.API.Repositories.Interfaces
{
    public interface IPeriodRepository
    {
        Task<IEnumerable<Period>> GetAllAsync();
        Task<Period?> GetByIdAsync(int id);
        Task<Period> AddAsync(Period period);
        Task UpdateAsync(Period period);
        Task DeleteAsync(int id);
    }
}
