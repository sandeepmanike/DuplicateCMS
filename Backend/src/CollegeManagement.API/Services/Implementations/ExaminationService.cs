using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using CollegeManagement.API.DTOs.Examination.Requests;
using CollegeManagement.API.DTOs.Examination.Responses;
using CollegeManagement.API.Exceptions;
using CollegeManagement.API.Models;
using CollegeManagement.API.Repositories.Interfaces;
using CollegeManagement.API.Services.Interfaces;

namespace CollegeManagement.API.Services.Implementations
{
    public class ExaminationService : IExaminationService
    {
        private readonly IExaminationRepository _examinationRepository;
        private readonly IMapper _mapper;

        public ExaminationService(IExaminationRepository examinationRepository, IMapper mapper)
        {
            _examinationRepository = examinationRepository;
            _mapper = mapper;
        }

        #region Examination Implementations

        public async Task<ExaminationResponse> CreateExaminationAsync(CreateExaminationRequest request)
        {
            if (request.EndDate < request.StartDate)
            {
                throw new ValidationException("End Date cannot be earlier than Start Date.");
            }

            var exam = _mapper.Map<Examination>(request);
            var createdExam = await _examinationRepository.CreateExaminationAsync(exam);

            var fullyLoadedExam = await _examinationRepository.GetExaminationByIdAsync(createdExam.ExaminationId);
            if (fullyLoadedExam == null)
            {
                throw new InvalidOperationException("Unable to retrieve created examination.");
            }

            return _mapper.Map<ExaminationResponse>(fullyLoadedExam);
        }

        public async Task<ExaminationResponse?> GetExaminationByIdAsync(int examinationId)
        {
            var exam = await _examinationRepository.GetExaminationByIdAsync(examinationId);
            return exam == null ? null : _mapper.Map<ExaminationResponse>(exam);
        }

        public async Task<IEnumerable<ExaminationResponse>> GetExaminationsAsync(string? courseId)
        {
            var exams = await _examinationRepository.GetExaminationsAsync(courseId);
            return _mapper.Map<IEnumerable<ExaminationResponse>>(exams);
        }

        public async Task<ExaminationResponse?> UpdateExaminationAsync(int examinationId, UpdateExaminationRequest request)
        {
            var exam = await _examinationRepository.GetExaminationByIdAsync(examinationId);
            if (exam == null) return null;

            _mapper.Map(request, exam);
            await _examinationRepository.UpdateExaminationAsync(exam);

            return _mapper.Map<ExaminationResponse>(exam);
        }

        public async Task<bool> DeleteExaminationAsync(int examinationId)
        {
            var exam = await _examinationRepository.GetExaminationByIdAsync(examinationId);
            if (exam == null) return false;

            return await _examinationRepository.DeleteExaminationAsync(exam);
        }

        public async Task<ExaminationStatusResponse?> CancelExaminationAsync(int examinationId, CancelExaminationRequest request)
        {
            var exam = await _examinationRepository.GetExaminationByIdAsync(examinationId);
            if (exam == null) return null;

            exam.Status = "CANCELLED";
            await _examinationRepository.UpdateExaminationAsync(exam);

            return new ExaminationStatusResponse
            {
                ExaminationId = exam.ExaminationId,
                Status = exam.Status,
                ActionReason = request.Reason,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public async Task<ExaminationStatusResponse?> RescheduleExaminationAsync(int examinationId, RescheduleExaminationRequest request)
        {
            var exam = await _examinationRepository.GetExaminationByIdAsync(examinationId);
            if (exam == null) return null;

            exam.Status = "RESCHEDULED";
            await _examinationRepository.UpdateExaminationAsync(exam);

            return new ExaminationStatusResponse
            {
                ExaminationId = exam.ExaminationId,
                Status = exam.Status,
                ActionReason = request.Reason,
                UpdatedAt = DateTime.UtcNow
            };
        }

        #endregion

        #region Exam Schedule Implementations

        public async Task<ExamScheduleResponse> CreateExamScheduleAsync(CreateExamScheduleRequest request)
        {
            var schedule = _mapper.Map<ExamSchedule>(request);
            var createdSchedule = await _examinationRepository.CreateExamScheduleAsync(schedule);

            var fullyLoadedSchedule = await _examinationRepository.GetExamScheduleByIdAsync(createdSchedule.ExamScheduleId);
            if (fullyLoadedSchedule == null)
            {
                throw new InvalidOperationException("Unable to retrieve created exam schedule.");
            }

            return _mapper.Map<ExamScheduleResponse>(fullyLoadedSchedule);
        }

        public async Task<ExamScheduleResponse?> GetExamScheduleByIdAsync(int examScheduleId)
        {
            var schedule = await _examinationRepository.GetExamScheduleByIdAsync(examScheduleId);
            return schedule == null ? null : _mapper.Map<ExamScheduleResponse>(schedule);
        }

        public async Task<IEnumerable<ExamScheduleResponse>> GetExamSchedulesAsync(int? examinationId)
        {
            var schedules = await _examinationRepository.GetExamSchedulesAsync(examinationId);
            return _mapper.Map<IEnumerable<ExamScheduleResponse>>(schedules);
        }

        public async Task<ExamScheduleResponse?> UpdateExamScheduleAsync(int examScheduleId, UpdateExamScheduleRequest request)
        {
            var schedule = await _examinationRepository.GetExamScheduleByIdAsync(examScheduleId);
            if (schedule == null) return null;

            _mapper.Map(request, schedule);
            await _examinationRepository.UpdateExamScheduleAsync(schedule);

            return _mapper.Map<ExamScheduleResponse>(schedule);
        }

        public async Task<int> PublishExamSchedulesAsync(PublishExamScheduleRequest request)
        {
            return await _examinationRepository.PublishExamSchedulesAsync(request.ScheduleIds);
        }

        #endregion

        #region Hall Ticket Implementations

        public async Task<IEnumerable<HallTicketResponse>> GenerateHallTicketsAsync(GenerateHallTicketRequest request)
        {
            var hallTickets = await _examinationRepository.GenerateHallTicketsAsync(request.ExaminationId, request.BatchId);
            return _mapper.Map<IEnumerable<HallTicketResponse>>(hallTickets);
        }

        public async Task<Stream?> DownloadHallTicketPdfAsync(int studentId, int examinationId)
        {
            return await _examinationRepository.GetHallTicketPdfStreamAsync(studentId, examinationId);
        }

        #endregion

        #region Invigilator Implementations

        public async Task AssignInvigilatorsAsync(AssignInvigilatorRequest request)
        {
            await _examinationRepository.AssignInvigilatorsAsync(request.ExamScheduleId, request.InvigilatorIds, request.HallNumber);
        }

        public async Task<IEnumerable<InvigilatorAssignmentResponse>> GetInvigilatorsAsync(int examScheduleId)
        {
            var assignments = await _examinationRepository.GetInvigilatorsByScheduleIdAsync(examScheduleId);
            return _mapper.Map<IEnumerable<InvigilatorAssignmentResponse>>(assignments);
        }

        #endregion
    }
}