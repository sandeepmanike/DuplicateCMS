using System.Collections.Generic;

namespace CollegeManagement.API.DTOs.Board.Responses
{
    public class BoardFormDataResponse
    {
        public IEnumerable<CountryResponse> Countries { get; set; } = new List<CountryResponse>();
        public IEnumerable<AcademicPatternResponse> AcademicPatterns { get; set; } = new List<AcademicPatternResponse>();
        public IEnumerable<AcademicLevelResponse> AcademicLevels { get; set; } = new List<AcademicLevelResponse>();
        public IEnumerable<GradingSystemResponse> GradingSystems { get; set; } = new List<GradingSystemResponse>();
    }
}
