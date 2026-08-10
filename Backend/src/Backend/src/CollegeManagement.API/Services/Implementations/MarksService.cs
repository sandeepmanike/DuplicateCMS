using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CollegeManagement.API.DTOs.Marks;
using CollegeManagement.API.Exceptions;
using CollegeManagement.API.Models;
using CollegeManagement.API.Repositories.Interfaces;
using CollegeManagement.API.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace CollegeManagement.API.Services.Implementations
{
    public class MarksService : IMarksService
    {
        private readonly IMarksRepository _marksRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MarksService> _logger;

        public MarksService(IMarksRepository marksRepository, IMapper mapper, ILogger<MarksService> logger)
        {
            _marksRepository = marksRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<MarkResponseDto>> GetAllMarksAsync()
        {
            var marks = await _marksRepository.GetAllAsync();
            return _mapper.Map<List<MarkResponseDto>>(marks ?? new List<Mark>());
        }

        public async Task<MarkResponseDto> GetMarkByIdAsync(int id)
        {
            var mark = await _marksRepository.GetByIdAsync(id);
            if (mark == null) throw new NotFoundException($"Mark record with ID {id} not found.");
            return _mapper.Map<MarkResponseDto>(mark);
        }

        public async Task<MarkResponseDto> SaveMarkAsync(SaveMarkDto dto)
        {
            var markEntity = _mapper.Map<Mark>(dto);
            var created = await _marksRepository.CreateAsync(markEntity);
            if (created == null) throw new ValidationException("Failed to save mark entry.");
            return _mapper.Map<MarkResponseDto>(created);
        }

        public async Task<List<MarkResponseDto>> BulkSaveMarksAsync(BulkUploadMarksDto dto)
        {
            var result = new List<MarkResponseDto>();
            if (dto?.Marks != null && dto.Marks.Any())
            {
                foreach (var item in dto.Marks)
                {
                    try
                    {
                        var saved = await SaveMarkAsync(item);
                        if (saved != null)
                        {
                            result.Add(saved);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occurred while bulk saving mark for StudentId: {StudentId}", item.StudentId);
                    }
                }
            }
            return result;
        }

        public async Task<MarkResponseDto> UpdateMarkAsync(int id, UpdateMarkDto dto)
        {
            var existing = await _marksRepository.GetByIdAsync(id);
            if (existing == null) throw new NotFoundException($"Mark record with ID {id} not found.");
            if (existing.IsPublished) throw new ConflictException("Cannot edit published marks.");
            _mapper.Map(dto, existing);
            var updated = await _marksRepository.UpdateAsync(id, existing);
            return _mapper.Map<MarkResponseDto>(updated ?? existing);
        }

        public async Task<bool> DeleteMarkAsync(int id)
        {
            var existing = await _marksRepository.GetByIdAsync(id);
            if (existing == null) throw new NotFoundException($"Mark record with ID {id} not found.");
            if (existing.IsPublished) throw new ConflictException("Cannot delete published marks.");
            return await _marksRepository.DeleteAsync(id);
        }

        public async Task<bool> RestoreMarkAsync(int id)
        {
            return await _marksRepository.RestoreAsync(id);
        }

        public async Task<List<MarkResponseDto>> GetMarksByStudentAsync(int studentId)
        {
            var marks = await _marksRepository.GetByStudentAsync(studentId);
            return _mapper.Map<List<MarkResponseDto>>(marks ?? new List<Mark>());
        }

        public async Task<List<MarkResponseDto>> GetMarksBySubjectAsync(int subjectId)
        {
            var marks = await _marksRepository.GetBySubjectAsync(subjectId);
            return _mapper.Map<List<MarkResponseDto>>(marks ?? new List<Mark>());
        }

        public async Task<List<MarkResponseDto>> GetMarksByExamAsync(int examinationId)
        {
            var marks = await _marksRepository.GetByExamAsync(examinationId);
            return _mapper.Map<List<MarkResponseDto>>(marks ?? new List<Mark>());
        }

        public async Task<int> VerifyMarksAsync(VerifyMarksDto dto)
        {
            return await _marksRepository.VerifyMarksAsync(dto.ExaminationId, dto.SubjectId, dto.SectionId, dto.VerifiedBy);
        }

        public async Task<int> PublishMarksAsync(PublishMarksDto dto)
        {
            return await _marksRepository.PublishMarksAsync(dto.ExaminationId, dto.SubjectId, dto.SectionId);
        }

        public async Task<MarksSummaryDto> GetSummaryAsync(int examinationId)
        {
            var marks = await _marksRepository.GetByExamAsync(examinationId);

            if (marks == null || !marks.Any())
            {
                return new MarksSummaryDto
                {
                    TotalStudents = 0,
                    TotalMarksEntered = 0,
                    VerifiedStudents = 0,
                    PendingStudents = 0,
                    PassedStudents = 0,
                    FailedStudents = 0,
                    PassPercentage = 0,
                    HighestMarks = 0,
                    AverageMarks = 0
                };
            }

            var total = marks.Count;
            var verified = marks.Count(m => m.IsVerified);
            var pending = total - verified;
            var passed = marks.Count(m => (m.InternalMarks + m.PracticalMarks + m.TheoryMarks) >= m.PassingMarks);

            return new MarksSummaryDto
            {
                TotalStudents = total,
                TotalMarksEntered = total,
                VerifiedStudents = verified,
                PendingStudents = pending,
                PassedStudents = passed,
                FailedStudents = total - passed,
                PassPercentage = Math.Round(((decimal)passed / total) * 100, 2),
                HighestMarks = marks.Max(m => m.InternalMarks + m.PracticalMarks + m.TheoryMarks),
                AverageMarks = Math.Round((decimal)marks.Average(m => m.InternalMarks + m.PracticalMarks + m.TheoryMarks), 2)
            };
        }

        public async Task<byte[]> ExportCsvAsync(int examinationId, int subjectId)
        {
            var marks = await _marksRepository.GetByExamAsync(examinationId);
            var filtered = marks != null
                ? marks.Where(m => m.SubjectId == subjectId).ToList()
                : new List<Mark>();

            var sb = new StringBuilder();
            sb.AppendLine("RollNo,StudentName,Internal,Practical,Theory,Total,Status");

            foreach (var m in filtered)
            {
                var total = m.InternalMarks + m.PracticalMarks + m.TheoryMarks;
                var status = total >= m.PassingMarks ? "Pass" : "Fail";
                sb.AppendLine($"{m.RollNo},{m.StudentName},{m.InternalMarks},{m.PracticalMarks},{m.TheoryMarks},{total},{status}");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}