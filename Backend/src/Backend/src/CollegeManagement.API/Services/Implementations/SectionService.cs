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

        public async Task<IEnumerable<SectionResponse>> GetAllSectionsAsync()
        {
            return await _sectionRepository.GetAllSectionsAsync();
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
            // 1. Validate AcademicYearId exists
            if (!await _sectionRepository.AcademicYearExistsAsync(request.AcademicYearId))
            {
                throw new NotFoundException($"Academic Year with ID {request.AcademicYearId} does not exist.");
            }

            // 2. Validate ClassTeacherId exists (if provided)
            if (request.ClassTeacherId.HasValue && !await _sectionRepository.FacultyExistsAsync(request.ClassTeacherId.Value))
            {
                throw new NotFoundException($"Faculty (Class Teacher) with ID {request.ClassTeacherId.Value} does not exist.");
            }

            // 3. Validate duplicate Section Name in the same context
            if (await _sectionRepository.IsSectionNameDuplicateAsync(
                request.Board, request.AcademicYearId, request.Group, request.AcademicLevel, request.SectionName))
            {
                throw new ConflictException($"A section named '{request.SectionName}' already exists for this Board, Academic Year, Group, and Academic Level configuration.");
            }

            // 4. Map DTO to Entity and insert
            var section = _mapper.Map<Section>(request);
            var sectionId = await _sectionRepository.CreateSectionAsync(section);

            // 5. Retrieve created Section details
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

            // 2. Validate AcademicYearId exists
            if (!await _sectionRepository.AcademicYearExistsAsync(request.AcademicYearId))
            {
                throw new NotFoundException($"Academic Year with ID {request.AcademicYearId} does not exist.");
            }

            // 3. Validate ClassTeacherId exists (if provided)
            if (request.ClassTeacherId.HasValue && !await _sectionRepository.FacultyExistsAsync(request.ClassTeacherId.Value))
            {
                throw new NotFoundException($"Faculty (Class Teacher) with ID {request.ClassTeacherId.Value} does not exist.");
            }

            // 4. Validate duplicate Section Name (excluding current Section)
            if (await _sectionRepository.IsSectionNameDuplicateAsync(
                request.Board, request.AcademicYearId, request.Group, request.AcademicLevel, request.SectionName, id))
            {
                throw new ConflictException($"A section named '{request.SectionName}' already exists for this Board, Academic Year, Group, and Academic Level configuration.");
            }

            // 5. Map DTO to Entity and update
            var section = _mapper.Map<Section>(request);
            var updated = await _sectionRepository.UpdateSectionAsync(id, section);
            if (!updated)
            {
                throw new InvalidOperationException("Failed to update section.");
            }

            // 6. Retrieve updated details
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
            // Note: If needed, we can validate the Group exists here, but sp_GetSectionsByGroup handles checking matched groups
            return await _sectionRepository.GetSectionsByGroupAsync(groupId);
        }
    }
}
