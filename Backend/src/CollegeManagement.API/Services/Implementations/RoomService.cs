using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CollegeManagement.API.DTOs.Timetable;
using CollegeManagement.API.Models.Timetable;
using CollegeManagement.API.Repositories.Interfaces;
using CollegeManagement.API.Services.Interfaces;

namespace CollegeManagement.API.Services.Implementations
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IMapper _mapper;

        public RoomService(IRoomRepository roomRepository, IMapper mapper)
        {
            _roomRepository = roomRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RoomResponseDto>> GetAllAsync(RoomFilterDto? filter = null)
        {
            var rooms = await _roomRepository.GetAllFilteredAsync(filter);
            return _mapper.Map<IEnumerable<RoomResponseDto>>(rooms);
        }

        public async Task<RoomResponseDto?> GetByIdAsync(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            return room == null ? null : _mapper.Map<RoomResponseDto>(room);
        }

        public async Task<RoomResponseDto> CreateAsync(CreateRoomDto dto)
        {
            var effectiveCode = (dto.RoomCode ?? dto.RoomNumber ?? dto.RoomName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(effectiveCode))
            {
                throw new InvalidOperationException("Room number or room code is required.");
            }

            var existingByCode = await _roomRepository.GetByCodeAsync(effectiveCode);
            if (existingByCode != null)
            {
                throw new InvalidOperationException($"Room with code '{effectiveCode}' already exists.");
            }

            var entity = _mapper.Map<Room>(dto);
            entity.RoomCode = effectiveCode;
            entity.RoomNumber = effectiveCode;
            entity.RoomName = !string.IsNullOrWhiteSpace(dto.RoomName) ? dto.RoomName.Trim() : effectiveCode;
            entity.BlockName = !string.IsNullOrWhiteSpace(dto.BlockName) ? dto.BlockName.Trim() : (!string.IsNullOrWhiteSpace(dto.Building) ? dto.Building.Trim() : string.Empty);
            entity.Capacity = dto.Capacity > 0 ? dto.Capacity : 60;
            entity.RoomType = !string.IsNullOrWhiteSpace(dto.RoomType) ? dto.RoomType.Trim() : "Classroom";
            entity.IsActive = dto.IsActive;

            var result = await _roomRepository.AddAsync(entity);
            return _mapper.Map<RoomResponseDto>(result);
        }

        public async Task<RoomResponseDto?> UpdateAsync(int id, UpdateRoomDto dto)
        {
            var existing = await _roomRepository.GetByIdAsync(id);
            if (existing == null) return null;

            var effectiveCode = (dto.RoomCode ?? dto.RoomNumber ?? dto.RoomName ?? existing.RoomCode ?? existing.RoomNumber ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(effectiveCode))
            {
                throw new InvalidOperationException("Room number or room code is required.");
            }

            var existingByCode = await _roomRepository.GetByCodeAsync(effectiveCode);
            if (existingByCode != null && existingByCode.RoomId != id)
            {
                throw new InvalidOperationException($"Room with code '{effectiveCode}' already exists.");
            }

            // Check active section assignments for protection safeguards
            var assignedSections = (await _roomRepository.GetAssignedActiveSectionsByRoomAsync(id, existing.RoomCode ?? existing.RoomNumber)).ToList();
            if (assignedSections.Count > 0)
            {
                var existingCode = (existing.RoomCode ?? existing.RoomNumber ?? string.Empty).Trim();
                var newCode = effectiveCode;
                if (!string.Equals(existingCode, newCode, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Room code cannot be changed because this room is assigned to an active section.");
                }

                if (string.Equals(existing.RoomType, "Classroom", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(dto.RoomType?.Trim(), "Classroom", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Room type cannot be changed because this room is assigned to an active section.");
                }

                var maxStrength = assignedSections.Max(s => s.MaximumStrength);
                if (dto.Capacity < maxStrength)
                {
                    throw new InvalidOperationException("Room capacity cannot be reduced below the strength of an assigned active section.");
                }

                if (existing.IsActive && !dto.IsActive)
                {
                    throw new InvalidOperationException("Cannot deactivate this room because it is assigned to an active section.");
                }
            }

            _mapper.Map(dto, existing);
            existing.RoomCode = effectiveCode;
            existing.RoomNumber = effectiveCode;
            existing.RoomName = !string.IsNullOrWhiteSpace(dto.RoomName) ? dto.RoomName.Trim() : effectiveCode;
            existing.BlockName = !string.IsNullOrWhiteSpace(dto.BlockName) ? dto.BlockName.Trim() : (!string.IsNullOrWhiteSpace(dto.Building) ? dto.Building.Trim() : existing.BlockName);
            if (dto.Capacity > 0) existing.Capacity = dto.Capacity;
            if (!string.IsNullOrWhiteSpace(dto.RoomType)) existing.RoomType = dto.RoomType.Trim();
            existing.IsActive = dto.IsActive;

            await _roomRepository.UpdateAsync(existing);
            return _mapper.Map<RoomResponseDto>(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _roomRepository.GetByIdAsync(id);
            if (existing == null) return false;

            var assignedSections = (await _roomRepository.GetAssignedActiveSectionsByRoomAsync(id, existing.RoomCode ?? existing.RoomNumber)).ToList();
            if (assignedSections.Count > 0)
            {
                throw new InvalidOperationException("Cannot delete this room because it is assigned to an active section.");
            }

            await _roomRepository.DeleteAsync(id);
            return true;
        }
    }
}
