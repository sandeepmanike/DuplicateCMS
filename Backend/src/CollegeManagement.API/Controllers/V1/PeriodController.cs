using System.Collections.Generic;
using System.Threading.Tasks;
using CollegeManagement.API.DTOs.Timetable;
using CollegeManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollegeManagement.API.Controllers.V1
{
    [ApiController]
    [Route("api/v1/periods")]
    public class PeriodController : ControllerBase
    {
        private readonly IPeriodService _periodService;

        public PeriodController(IPeriodService periodService)
        {
            _periodService = periodService;
        }

        /// <summary>
        /// Gets all active periods sorted by display order.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PeriodResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _periodService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Gets a period by ID.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(PeriodResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _periodService.GetByIdAsync(id);
            if (result == null) return NotFound(new { message = $"Period with ID {id} not found." });
            return Ok(result);
        }

        /// <summary>
        /// Creates a new master period slot.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(PeriodResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreatePeriodDto dto)
        {
            var result = await _periodService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.PeriodId }, result);
        }

        /// <summary>
        /// Updates an existing period slot.
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(PeriodResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePeriodDto dto)
        {
            var result = await _periodService.UpdateAsync(id, dto);
            if (result == null) return NotFound(new { message = $"Period with ID {id} not found." });
            return Ok(result);
        }

        /// <summary>
        /// Deletes a period slot.
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _periodService.DeleteAsync(id);
            if (!success) return NotFound(new { message = $"Period with ID {id} not found." });
            return NoContent();
        }
    }
}
