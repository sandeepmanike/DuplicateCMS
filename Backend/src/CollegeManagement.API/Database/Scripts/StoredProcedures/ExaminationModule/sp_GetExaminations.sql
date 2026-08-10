DROP PROCEDURE IF EXISTS sp_GetExaminations;
DELIMITER //
CREATE PROCEDURE sp_GetExaminations()
BEGIN
    SELECT 
        e.ExamId,
        e.ExamName,
        e.BoardId,
        b.BoardName,
        e.AcademicYearId,
        ay.AcademicYearName,
        e.AcademicLevelId,
        e.GroupId,
        g.GroupName,
        e.AssessmentTypeId,
        e.StartDate,
        e.EndDate,
        e.Status,
        e.IsActive,
        e.CreatedAt
    FROM Examinations e
    LEFT JOIN Boards b ON b.BoardId = e.BoardId
    LEFT JOIN AcademicYears ay ON ay.AcademicYearId = e.AcademicYearId
    LEFT JOIN `Groups` g ON g.GroupId = e.GroupId
    ORDER BY e.ExamId DESC;
END //
DELIMITER ;
