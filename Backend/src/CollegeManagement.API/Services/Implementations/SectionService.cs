using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CollegeManagement.API.DTOs.Sections;
using CollegeManagement.API.Exceptions;
using CollegeManagement.API.Models;
using CollegeManagement.API.Repositories.Interfaces;
using CollegeManagement.API.Services.Interfaces;

namespace CollegeManagement.API.Services.Implementations
{
    public class SectionService : ISectionService
    {
        private readonly ISectionRepository _sectionRepository;
        private readonly IMapper _mapper;

        public SectionService(ISectionRepository sectionRepository, IMapper mapper)
        {
            _sectionRepository = sectionRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SectionResponse>> GetAllSectionsAsync(SectionFilterDto? filter = null)
        {
            return await _sectionRepository.GetAllSectionsAsync(filter);
        }

        public async Task<SectionResponse?> GetSectionByIdAsync(int id)
        {
            var section = await _sectionRepository.GetSectionByIdAsync(id);
            if (section == null)
            {
                throw new NotFoundException($"Section with ID {id} not found.");
            }
            return section;
        }

        public async Task<SectionResponse> CreateSectionAsync(CreateSectionRequest request)
        {
            // Normalize section name
            request.SectionName = NormalizeSectionName(request.SectionName);

            // 1. Validate AcademicYear exists, is active, and has not ended
            await ValidateAcademicYearAsync(request.AcademicYearId);

            // 2. Validate Incharge exists (if provided)
            var inchargeId = request.InchargeId ?? request.ClassTeacherId;
            if (inchargeId.HasValue && inchargeId.Value > 0)
            {
                if (!await _sectionRepository.FacultyExistsAsync(inchargeId.Value))
                {
                    throw new NotFoundException($"Faculty (Incharge) with ID {inchargeId.Value} does not exist.");
                }
            }

            // 3. Validate Room Allotment, Type, Capacity & Clash (when Active)
            await ValidateRoomAllotmentAsync(request.RoomId, request.RoomNumber, request.MaximumStrength, request.IsActive, null,
                (id, code) =>
                {
                    request.RoomId = id;
                    request.RoomNumber = code;
                });

            // 4. Validate duplicate Section Name in the same context
            if (await _sectionRepository.IsSectionNameDuplicateAsync(
                request.Board, request.AcademicYearId, request.Group, request.Programme, request.AcademicLevel, request.SectionName))
            {
                throw new ConflictException($"A section named '{request.SectionName}' already exists for this Board, Academic Year, Group, Programme, and Level configuration.");
            }

            // 5. Map DTO to Entity and insert
            var section = _mapper.Map<Section>(request);
            var sectionId = await _sectionRepository.CreateSectionAsync(section);

            // 6. Retrieve created Section details
            var createdSection = await _sectionRepository.GetSectionByIdAsync(sectionId);
            if (createdSection == null)
            {
                throw new InvalidOperationException("Failed to retrieve created section details.");
            }
            return createdSection;
        }

        public async Task<SectionResponse> UpdateSectionAsync(int id, UpdateSectionRequest request)
        {
            // 1. Verify existence of Section
            var existingSection = await _sectionRepository.GetSectionByIdAsync(id);
            if (existingSection == null)
            {
                throw new NotFoundException($"Section with ID {id} not found.");
            }

            // Normalize section name
            request.SectionName = NormalizeSectionName(request.SectionName);

            // 2. Validate AcademicYear exists, is active, and has not ended
            await ValidateAcademicYearAsync(request.AcademicYearId);

            // 3. Validate Incharge exists (if provided)
            var inchargeId = request.InchargeId ?? request.ClassTeacherId;
            if (inchargeId.HasValue && inchargeId.Value > 0)
            {
                if (!await _sectionRepository.FacultyExistsAsync(inchargeId.Value))
                {
                    throw new NotFoundException($"Faculty (Incharge) with ID {inchargeId.Value} does not exist.");
                }
            }

            // 4. Validate Room Allotment, Type, Capacity & Clash (when Active)
            await ValidateRoomAllotmentAsync(request.RoomId, request.RoomNumber, request.MaximumStrength, request.IsActive, id,
                (roomId, code) =>
                {
                    request.RoomId = roomId;
                    request.RoomNumber = code;
                });

            // 5. Validate duplicate Section Name (excluding current Section)
            if (await _sectionRepository.IsSectionNameDuplicateAsync(
                request.Board, request.AcademicYearId, request.Group, request.Programme, request.AcademicLevel, request.SectionName, id))
            {
                throw new ConflictException($"A section named '{request.SectionName}' already exists for this Board, Academic Year, Group, Programme, and Level configuration.");
            }

            // 6. Map DTO to Entity and update
            var section = _mapper.Map<Section>(request);
            var updated = await _sectionRepository.UpdateSectionAsync(id, section);
            if (!updated)
            {
                throw new InvalidOperationException("Failed to update section.");
            }

            // 7. Retrieve updated details
            var updatedSection = await _sectionRepository.GetSectionByIdAsync(id);
            if (updatedSection == null)
            {
                throw new NotFoundException($"Section with ID {id} not found after update.");
            }
            return updatedSection;
        }

        public async Task<bool> DeleteSectionAsync(int id)
        {
            var existingSection = await _sectionRepository.GetSectionByIdAsync(id);
            if (existingSection == null)
            {
                throw new NotFoundException($"Section with ID {id} not found.");
            }

            return await _sectionRepository.DeleteSectionAsync(id);
        }

        public async Task<IEnumerable<SectionResponse>> GetSectionsByGroupAsync(int groupId)
        {
            return await _sectionRepository.GetSectionsByGroupAsync(groupId);
        }

        private static string NormalizeSectionName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return string.Empty;
            return System.Text.RegularExpressions.Regex.Replace(name.Trim(), @"\s+", " ");
        }

        private async Task ValidateRoomAllotmentAsync(
            int? roomId,
            string? roomNumber,
            int sectionStrength,
            bool isSectionActive,
            int? excludeSectionId,
            Action<int?, string?> setSynchronizedRoom)
        {
            if (!roomId.HasValue && string.IsNullOrWhiteSpace(roomNumber))
            {
                return;
            }

            var room = await _sectionRepository.GetRoomDetailsAsync(roomId, roomNumber);
            if (room == null)
            {
                throw new NotFoundException("Selected room does not exist.");
            }

            setSynchronizedRoom(room.RoomId, room.RoomCode ?? room.RoomNumber);

            if (isSectionActive)
            {
                if (!room.IsActive)
                {
                    throw new ValidationException("Please select an active classroom room.");
                }

                if (!string.Equals(room.RoomType, "Classroom", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ValidationException("Only Classroom rooms can be assigned to a section.");
                }

                if (sectionStrength > room.Capacity)
                {
                    throw new ValidationException($"Section capacity cannot exceed room capacity ({room.Capacity}).");
                }

                var conflictingSection = await _sectionRepository.GetActiveSectionAssignedToRoomAsync(room.RoomId, room.RoomCode, excludeSectionId);
                if (conflictingSection != null)
                {
                    throw new ConflictException("Selected room is already assigned to another active section.");
                }
            }
        }

        private async Task ValidateAcademicYearAsync(int academicYearId)
        {
            var academicYear = await _sectionRepository.GetAcademicYearByIdAsync(academicYearId);
            if (academicYear == null)
            {
                throw new NotFoundException($"Academic Year with ID {academicYearId} does not exist.");
            }

            if (!academicYear.IsActive)
            {
                throw new ValidationException($"Academic Year '{academicYear.AcademicYearName}' is inactive and cannot be assigned to new sections.");
            }

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (academicYear.EndDate < today)
            {
                throw new ValidationException($"Academic Year '{academicYear.AcademicYearName}' ended on {academicYear.EndDate:yyyy-MM-dd} and cannot be assigned to new sections.");
            }
        }
    }
}
