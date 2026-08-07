using System.Collections.Generic;
using System.Threading.Tasks;
using CollegeManagement.API.Models;

namespace CollegeManagement.API.Repositories.Interfaces
{
    public interface ISectionRepository
    {
        Task<IEnumerable<Section>> GetActiveSectionsAsync();
    }
}
