using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CollegeManagement.API.Data;
using CollegeManagement.API.Models;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly AppDbContext _context;

        public SubjectRepository(AppDbContext context) => _context = context;

        private static string Clean(string value) => string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();

        public async Task<IEnumerable<Subject>> GetAllAsync()
        {
            return await _context.Subjects
                .Include(s => s.BoardNavigation)
                .Include(s => s.GroupNavigation)
                .Include(s => s.AcademicLevelNavigation)
                .OrderBy(s => s.SubjectId)
                .ToListAsync();
        }

        public async Task<Subject?> GetByIdAsync(int subjectId)
        {
            return await _context.Subjects
                .Include(s => s.BoardNavigation)
                .Include(s => s.GroupNavigation)
                .Include(s => s.AcademicLevelNavigation)
                .FirstOrDefaultAsync(s => s.SubjectId == subjectId);
        }

        public async Task<Subject> CreateAsync(Subject subject)
        {
            subject.SubjectName = Clean(subject.SubjectName);
            subject.SubjectCode = Clean(subject.SubjectCode);
            subject.SubjectType = Clean(subject.SubjectType);
            subject.CreatedAt = DateTime.UtcNow;

            await _context.Subjects.AddAsync(subject);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(subject.SubjectId) ?? subject;
        }

        public async Task<Subject?> UpdateAsync(int subjectId, Subject subject)
        {
            var existing = await _context.Subjects.FirstOrDefaultAsync(s => s.SubjectId == subjectId);
            if (existing == null) return null;

            existing.BoardId = subject.BoardId;
            existing.GroupId = subject.GroupId;
            existing.AcademicLevelId = subject.AcademicLevelId;
            existing.SubjectName = Clean(subject.SubjectName);
            existing.SubjectCode = Clean(subject.SubjectCode);
            existing.SubjectType = Clean(subject.SubjectType);
            existing.Theory = subject.Theory;
            existing.Practical = subject.Practical;
            existing.Language = subject.Language;
            existing.Elective = subject.Elective;
            existing.InternalMarks = subject.InternalMarks;
            existing.PracticalMarks = subject.PracticalMarks;
            existing.ExternalMarks = subject.ExternalMarks;
            existing.TotalMarks = subject.TotalMarks;
            existing.PassingMarks = subject.PassingMarks;
            existing.IsActive = subject.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(subjectId);
        }

        public async Task<bool> DeleteAsync(int subjectId)
        {
            var existing = await _context.Subjects.FirstOrDefaultAsync(s => s.SubjectId == subjectId);
            if (existing == null) return false;

            _context.Subjects.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Subject>> GetByGroupIdAsync(int groupId)
        {
            return await _context.Subjects
                .Include(s => s.BoardNavigation)
                .Include(s => s.GroupNavigation)
                .Include(s => s.AcademicLevelNavigation)
                .Where(s => s.GroupId == groupId)
                .OrderBy(s => s.SubjectId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Subject>> GetByContextAsync(int boardId, int groupId, int academicLevelId)
        {
            return await _context.Subjects
                .Include(s => s.BoardNavigation)
                .Include(s => s.GroupNavigation)
                .Include(s => s.AcademicLevelNavigation)
                .Where(s => s.BoardId == boardId && s.GroupId == groupId && s.AcademicLevelId == academicLevelId)
                .OrderBy(s => s.SubjectId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Subject>> SearchAsync(string? search, int? boardId, int? groupId, int? academicLevelId, bool? isActive)
        {
            var query = _context.Subjects
                .Include(s => s.BoardNavigation)
                .Include(s => s.GroupNavigation)
                .Include(s => s.AcademicLevelNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var cleanSearch = search.Trim().ToLower();
                query = query.Where(s => s.SubjectName.ToLower().Contains(cleanSearch) || s.SubjectCode.ToLower().Contains(cleanSearch));
            }

            if (boardId.HasValue)
                query = query.Where(s => s.BoardId == boardId.Value);

            if (groupId.HasValue)
                query = query.Where(s => s.GroupId == groupId.Value);

            if (academicLevelId.HasValue)
                query = query.Where(s => s.AcademicLevelId == academicLevelId.Value);

            if (isActive.HasValue)
                query = query.Where(s => s.IsActive == isActive.Value);

            return await query.OrderBy(s => s.SubjectId).ToListAsync();
        }

        public async Task<IEnumerable<Subject>> GetActiveAsync()
        {
            return await _context.Subjects
                .Include(s => s.BoardNavigation)
                .Include(s => s.GroupNavigation)
                .Include(s => s.AcademicLevelNavigation)
                .Where(s => s.IsActive)
                .OrderBy(s => s.SubjectId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Subject>> GetByBoardIdAsync(int boardId)
        {
            return await _context.Subjects
                .Include(s => s.BoardNavigation)
                .Include(s => s.GroupNavigation)
                .Include(s => s.AcademicLevelNavigation)
                .Where(s => s.BoardId == boardId)
                .OrderBy(s => s.SubjectId)
                .ToListAsync();
        }

        public async Task<bool> SubjectCodeExistsAsync(string subjectCode, int boardId, int groupId, int academicLevelId, int? excludeSubjectId = null)
        {
            var cleanCode = subjectCode.Trim().ToLower();
            return await _context.Subjects.AnyAsync(s =>
                s.SubjectCode.ToLower() == cleanCode &&
                s.BoardId == boardId &&
                s.GroupId == groupId &&
                s.AcademicLevelId == academicLevelId &&
                (!excludeSubjectId.HasValue || s.SubjectId != excludeSubjectId.Value));
        }
    }
}