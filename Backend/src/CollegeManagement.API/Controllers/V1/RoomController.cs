using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CollegeManagement.API.DTOs.Timetable;
using CollegeManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollegeManagement.API.Controllers.V1
{
    [ApiController]
    [Route("api/v1/rooms")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        /// <summary>
        /// Gets all rooms with optional filtering (Building/Block, Floor, RoomType, Status, Search).
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RoomResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] RoomFilterDto? filter)
        {
            var result = await _roomService.GetAllAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// Gets a room by ID.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(RoomResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _roomService.GetByIdAsync(id);
            if (result == null) return NotFound(new { message = $"Room with ID {id} not found." });
            return Ok(result);
        }

        /// <summary>
        /// Creates a new room.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(RoomResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateRoomDto dto)
        {
            try
            {
                var result = await _roomService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.RoomId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates a room.
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(RoomResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRoomDto dto)
        {
            try
            {
                var result = await _roomService.UpdateAsync(id, dto);
                if (result == null) return NotFound(new { message = $"Room with ID {id} not found." });
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a room.
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _roomService.DeleteAsync(id);
            if (!success) return NotFound(new { message = $"Room with ID {id} not found." });
            return NoContent();
        }
    }
}
