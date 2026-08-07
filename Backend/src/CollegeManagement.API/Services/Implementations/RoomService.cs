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

        public async Task<IEnumerable<RoomResponseDto>> GetAllAsync()
        {
            var rooms = await _roomRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<RoomResponseDto>>(rooms);
        }

        public async Task<RoomResponseDto?> GetByIdAsync(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            return room == null ? null : _mapper.Map<RoomResponseDto>(room);
        }

        public async Task<RoomResponseDto> CreateAsync(CreateRoomDto dto)
        {
            var existingByCode = await _roomRepository.GetByCodeAsync(dto.RoomCode.Trim());
            if (existingByCode != null)
            {
                throw new InvalidOperationException($"Room with code '{dto.RoomCode}' already exists.");
            }

            var entity = _mapper.Map<Room>(dto);
            entity.RoomCode = entity.RoomCode.Trim();
            var result = await _roomRepository.AddAsync(entity);
            return _mapper.Map<RoomResponseDto>(result);
        }

        public async Task<RoomResponseDto?> UpdateAsync(int id, UpdateRoomDto dto)
        {
            var existing = await _roomRepository.GetByIdAsync(id);
            if (existing == null) return null;

            var existingByCode = await _roomRepository.GetByCodeAsync(dto.RoomCode.Trim());
            if (existingByCode != null && existingByCode.RoomId != id)
            {
                throw new InvalidOperationException($"Room with code '{dto.RoomCode}' already exists.");
            }

            _mapper.Map(dto, existing);
            existing.RoomCode = existing.RoomCode.Trim();
            await _roomRepository.UpdateAsync(existing);
            return _mapper.Map<RoomResponseDto>(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _roomRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _roomRepository.DeleteAsync(id);
            return true;
        }
    }
}
