using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs.Board.Requests;
using CollegeManagement.API.DTOs.Board.Responses;
using CollegeManagement.API.Models;
using CollegeManagement.API.Repositories.Interfaces;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Repositories.Implementations
{
    /// <summary>
    /// Repository implementation for Board database operations using Dapper and MySQL Stored Procedures.
    /// </summary>
    public class BoardRepository : IBoardRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoardRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public BoardRepository(AppDbContext context)
        {
            _context = context;
        }

        private IDbConnection Connection => _context.Database.GetDbConnection();

        /// <summary>
        /// Starts a database transaction on the underlying connection.
        /// </summary>
        public async Task<IDbTransaction> BeginTransactionAsync()
        {
            if (Connection.State == ConnectionState.Closed)
            {
                await _context.Database.OpenConnectionAsync();
            }
            return await Task.FromResult(Connection.BeginTransaction());
        }

        /// <summary>
        /// Creates a new Board in the database using sp_CreateBoard.
        /// </summary>
        public async Task<Board> CreateBoardAsync(Board board, IDbTransaction? transaction = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_BoardCode", board.BoardCode);
            parameters.Add("p_BoardName", board.BoardName);
            parameters.Add("p_Description", board.Description);
            parameters.Add("p_CountryId", board.CountryId);
            parameters.Add("p_StateId", board.StateId);
            parameters.Add("p_AcademicPatternId", board.AcademicPatternId);
            parameters.Add("p_GradingSystemId", board.GradingSystemId);
            parameters.Add("p_InternalAssessment", board.InternalAssessment);
            parameters.Add("p_PracticalExams", board.PracticalExams);
            parameters.Add("p_BoardExams", board.BoardExams);
            parameters.Add("p_PassPercentage", board.PassPercentage);
            parameters.Add("p_RankCalculation", board.RankCalculation);
            parameters.Add("p_IsActive", board.IsActive);


            var boardId = await Connection.ExecuteScalarAsync<int>(
                 "sp_CreateBoard",
                    parameters,
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure);

            board.BoardId = boardId;
            return board;
        }

        /// <summary>
        /// Updates an existing Board in the database using sp_UpdateBoard with optimistic concurrency.
        /// </summary>
        public async Task<(Board? Board, int AffectedRows)> UpdateBoardAsync(Board board, uint expectedVersion, IDbTransaction? transaction = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_BoardId", board.BoardId);
            parameters.Add("p_ExpectedVersion", expectedVersion);
            parameters.Add("p_BoardName", board.BoardName);
            parameters.Add("p_BoardCode", board.BoardCode);
            parameters.Add("p_Description", board.Description);
            parameters.Add("p_CountryId", board.CountryId);
            parameters.Add("p_StateId", board.StateId);
            parameters.Add("p_AcademicPatternId", board.AcademicPatternId);
            parameters.Add("p_GradingSystemId", board.GradingSystemId);
            parameters.Add("p_InternalAssessment", board.InternalAssessment);
            parameters.Add("p_PracticalExams", board.PracticalExams);
            parameters.Add("p_BoardExams", board.BoardExams);
            parameters.Add("p_PassPercentage", board.PassPercentage);
            parameters.Add("p_RankCalculation", board.RankCalculation);
            parameters.Add("p_IsActive", board.IsActive);
            parameters.Add("p_AffectedRows", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var updatedBoard = await Connection.QueryFirstOrDefaultAsync<Board>(
                "sp_UpdateBoard",
                parameters,
                transaction: transaction,
                commandType: CommandType.StoredProcedure);

            var affected = parameters.Get<int>("p_AffectedRows");
            return (affected > 0 ? updatedBoard : null, affected);
        }

        /// <summary>
        /// Performs soft delete of a Board using sp_DeleteBoard with optimistic concurrency.
        /// </summary>
        public async Task<int> DeleteBoardAsync(int boardId, uint expectedVersion, IDbTransaction? transaction = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_BoardId", boardId);
            parameters.Add("p_ExpectedVersion", expectedVersion);
            parameters.Add("p_AffectedRows", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await Connection.ExecuteAsync(
                "sp_DeleteBoard",
                parameters,
                transaction: transaction,
                commandType: CommandType.StoredProcedure);

            return parameters.Get<int>("p_AffectedRows");
        }

        /// <summary>
        /// Retrieves a Board by ID including relations, using the active transaction if available.
        /// </summary>
        public async Task<Board?> GetBoardByIdAsync(int boardId, IDbTransaction? transaction = null)
        {
            var sql = @"
                SELECT b.*, c.*, s.*, ap.*, gs.*
                FROM Boards b
                LEFT JOIN Countries c ON b.CountryId = c.CountryId
                LEFT JOIN States s ON b.StateId = s.StateId
                LEFT JOIN AcademicPatterns ap ON b.AcademicPatternId = ap.AcademicPatternId
                LEFT JOIN GradingSystems gs ON b.GradingSystemId = gs.GradingSystemId
                WHERE b.BoardId = @BoardId;

                SELECT bal.*, al.*
                FROM BoardAcademicLevels bal
                LEFT JOIN AcademicLevels al ON bal.AcademicLevelId = al.AcademicLevelId
                WHERE bal.BoardId = @BoardId;";

            using (var multi = await Connection.QueryMultipleAsync(sql, new { BoardId = boardId }, transaction))
            {
                var board = multi.Read<Board, Country, State, AcademicPattern, GradingSystem, Board>((b, c, s, ap, gs) =>
                {
                    b.Country = c;
                    b.State = s;
                    b.AcademicPattern = ap;
                    b.GradingSystem = gs;
                    return b;
                }, splitOn: "CountryId,StateId,AcademicPatternId,GradingSystemId").FirstOrDefault();

                if (board != null)
                {
                    var academicLevels = multi.Read<BoardAcademicLevel, AcademicLevel, BoardAcademicLevel>((bal, al) =>
                    {
                        bal.AcademicLevel = al;
                        return bal;
                    }, splitOn: "AcademicLevelId").ToList();

                    board.BoardAcademicLevels = academicLevels;
                }

                return board;
            }
        }

        /// <summary>
        /// Retrieves filtered list of Boards with pagination, searching, and sorting using sp_GetBoards.
        /// </summary>
        public async Task<(List<Board> Items, int TotalCount)> GetBoardsAsync(BoardSearchRequest request)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_Search", request.Search);
            parameters.Add("p_BoardName", request.BoardName);
            parameters.Add("p_BoardCode", request.BoardCode);
            parameters.Add("p_CountryId", request.CountryId);
            parameters.Add("p_StateId", request.StateId);
            parameters.Add("p_Status", request.Status);
            parameters.Add("p_SortBy", request.SortBy);
            parameters.Add("p_SortOrder", request.SortOrder);
            parameters.Add("p_PageNumber", request.PageNumber);
            parameters.Add("p_PageSize", request.PageSize);

            using (var multi = await Connection.QueryMultipleAsync(
                "sp_GetBoards",
                parameters,
                commandType: CommandType.StoredProcedure))
            {
                var totalCount = await multi.ReadFirstOrDefaultAsync<int>();
                var items = multi.Read<Board, Country, State, AcademicPattern, GradingSystem, Board>((b, c, s, ap, gs) =>
                {
                    b.Country = c;
                    b.State = s;
                    b.AcademicPattern = ap;
                    b.GradingSystem = gs;
                    b.CountryId = c?.CountryId ?? 0;
                    b.StateId = s?.StateId;
                    b.AcademicPatternId = ap?.AcademicPatternId ?? 0;
                    b.GradingSystemId = gs?.GradingSystemId ?? 0;
                    return b;
                }, splitOn: "CountryId,StateId,AcademicPatternId,GradingSystemId").ToList();

                return (items, totalCount);
            }
        }

        /// <summary>
        /// Changes status of a Board using sp_ChangeBoardStatus with optimistic concurrency.
        /// </summary>
        public async Task<int> ChangeBoardStatusAsync(int boardId, uint expectedVersion, bool status, IDbTransaction? transaction = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_BoardId", boardId);
            parameters.Add("p_ExpectedVersion", expectedVersion);
            parameters.Add("p_Status", status);
            parameters.Add("p_AffectedRows", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await Connection.ExecuteAsync(
                "sp_ChangeBoardStatus",
                parameters,
                transaction: transaction,
                commandType: CommandType.StoredProcedure);

            return parameters.Get<int>("p_AffectedRows");
        }

        /// <summary>
        /// Checks duplicate board code using sp_ValidateBoardCode.
        /// </summary>
        public async Task<bool> IsBoardCodeExistsAsync(string boardCode, int? boardId = null)
        {
            var exists = await Connection.ExecuteScalarAsync<int>(
                "sp_ValidateBoardCode",
                new { p_BoardCode = boardCode, p_ExcludeBoardId = boardId },
                commandType: CommandType.StoredProcedure);

            return exists > 0;
        }

        /// <summary>
        /// Retrieves active countries using sp_GetCountries.
        /// </summary>
        public async Task<List<Country>> GetCountriesAsync()
        {
            var countries = await Connection.QueryAsync<Country>(
                "sp_GetCountries",
                commandType: CommandType.StoredProcedure);
            return countries.ToList();
        }

        /// <summary>
        /// Retrieves active states for a country using sp_GetStatesByCountry.
        /// </summary>
        public async Task<List<State>> GetStatesByCountryAsync(int countryId)
        {
            var states = await Connection.QueryAsync<State>(
                "sp_GetStatesByCountry",
                new { p_CountryId = countryId },
                commandType: CommandType.StoredProcedure);
            return states.ToList();
        }

        /// <summary>
        /// Retrieves active academic patterns using sp_GetAcademicPatterns.
        /// </summary>
        public async Task<List<AcademicPattern>> GetAcademicPatternsAsync()
        {
            var patterns = await Connection.QueryAsync<AcademicPattern>(
                "sp_GetAcademicPatterns",
                commandType: CommandType.StoredProcedure);
            return patterns.ToList();
        }

        /// <summary>
        /// Retrieves active academic levels using sp_GetAcademicLevels.
        /// </summary>
        public async Task<List<AcademicLevel>> GetAcademicLevelsAsync()
        {
            var levels = await Connection.QueryAsync<AcademicLevel>(
                "sp_GetAcademicLevels",
                commandType: CommandType.StoredProcedure);
            return levels.ToList();
        }

        /// <summary>
        /// Retrieves active grading systems using sp_GetGradingSystems.
        /// </summary>
        public async Task<List<GradingSystem>> GetGradingSystemsAsync()
        {
            var systems = await Connection.QueryAsync<GradingSystem>(
                "sp_GetGradingSystems",
                commandType: CommandType.StoredProcedure);
            return systems.ToList();
        }

        /// <summary>
        /// Replaces academic levels mapping for a board using sp_ReplaceBoardAcademicLevels.
        /// </summary>
        public async Task ReplaceAcademicLevelsAsync(int boardId, List<int> academicLevelIds, IDbTransaction? transaction = null)
        {
            var academicLevelIdsCsv = string.Join(",", academicLevelIds);
            await Connection.ExecuteAsync(
                "sp_ReplaceBoardAcademicLevels",
                new { p_BoardId = boardId, p_AcademicLevelIds = academicLevelIdsCsv },
                transaction: transaction,
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Checks if an academic level exists using sp_AcademicLevelExists.
        /// </summary>
        public async Task<bool> AcademicLevelExistsAsync(int academicLevelId)
        {
            var exists = await Connection.ExecuteScalarAsync<int>(
                "sp_AcademicLevelExists",
                new { p_AcademicLevelId = academicLevelId },
                commandType: CommandType.StoredProcedure);

            return exists > 0;
        }

        /// <summary>
        /// Checks if a country exists using sp_CountryExists.
        /// </summary>
        public async Task<bool> CountryExistsAsync(int countryId)
        {
            var exists = await Connection.ExecuteScalarAsync<int>(
                "sp_CountryExists",
                new { p_CountryId = countryId },
                commandType: CommandType.StoredProcedure);

            return exists > 0;
        }

        /// <summary>
        /// Checks if a state exists using sp_StateExists.
        /// </summary>
        public async Task<bool> StateExistsAsync(int stateId)
        {
            var exists = await Connection.ExecuteScalarAsync<int>(
                "sp_StateExists",
                new { p_StateId = stateId },
                commandType: CommandType.StoredProcedure);

            return exists > 0;
        }

        /// <summary>
        /// Checks if an academic pattern exists using sp_AcademicPatternExists.
        /// </summary>
        public async Task<bool> AcademicPatternExistsAsync(int academicPatternId)
        {
            var exists = await Connection.ExecuteScalarAsync<int>(
                "sp_AcademicPatternExists",
                new { p_AcademicPatternId = academicPatternId },
                commandType: CommandType.StoredProcedure);

            return exists > 0;
        }

        /// <summary>
        /// Checks if a grading system exists using sp_GradingSystemExists.
        /// </summary>
        public async Task<bool> GradingSystemExistsAsync(int gradingSystemId)
        {
            var exists = await Connection.ExecuteScalarAsync<int>(
                "sp_GradingSystemExists",
                new { p_GradingSystemId = gradingSystemId },
                commandType: CommandType.StoredProcedure);

            return exists > 0;
        }

        /// <summary>
        /// Checks if a state belongs to a country using sp_StateBelongsToCountry.
        /// </summary>
        public async Task<bool> StateBelongsToCountryAsync(int stateId, int countryId)
        {
            var belongs = await Connection.ExecuteScalarAsync<int>(
                "sp_StateBelongsToCountry",
                new { p_StateId = stateId, p_CountryId = countryId },
                commandType: CommandType.StoredProcedure);

            return belongs > 0;
        }

        /// <summary>
        /// Checks if all academic levels exist using sp_AcademicLevelExists.
        /// </summary>
        public async Task<bool> AcademicLevelsExistAsync(IEnumerable<int> academicLevelIds)
        {
            var ids = academicLevelIds.Distinct().ToList();
            if (!ids.Any())
            {
                return true;
            }

            var activeLevels = await GetAcademicLevelsAsync();
            var activeLevelIds = activeLevels.Select(al => al.AcademicLevelId).ToHashSet();

            return ids.All(id => activeLevelIds.Contains(id));
        }

        /// <inheritdoc />
        public async Task<BoardSummaryResponse> GetDashboardSummaryAsync()
        {
            if (Connection.State == ConnectionState.Closed)
            {
                await _context.Database.OpenConnectionAsync();
            }

            var sql = @"
                SELECT COUNT(*) FROM Boards;
                SELECT COUNT(*) FROM Boards WHERE IsActive = 1;
                SELECT COUNT(*) FROM Boards WHERE IsActive = 0;

                SELECT c.CountryId AS Id, c.CountryName AS Name, COUNT(b.BoardId) AS Count
                FROM Countries c
                LEFT JOIN Boards b ON b.CountryId = c.CountryId
                WHERE c.IsActive = 1
                GROUP BY c.CountryId, c.CountryName
                HAVING COUNT(b.BoardId) > 0;

                SELECT ap.AcademicPatternId AS Id, ap.PatternName AS Name, COUNT(b.BoardId) AS Count
                FROM AcademicPatterns ap
                LEFT JOIN Boards b ON b.AcademicPatternId = ap.AcademicPatternId
                WHERE ap.IsActive = 1
                GROUP BY ap.AcademicPatternId, ap.PatternName
                HAVING COUNT(b.BoardId) > 0;

                SELECT gs.GradingSystemId AS Id, gs.GradingSystemName AS Name, COUNT(b.BoardId) AS Count
                FROM GradingSystems gs
                LEFT JOIN Boards b ON b.GradingSystemId = gs.GradingSystemId
                WHERE gs.IsActive = 1
                GROUP BY gs.GradingSystemId, gs.GradingSystemName
                HAVING COUNT(b.BoardId) > 0;

                SELECT b.BoardId, b.BoardCode, b.BoardName, c.CountryName, b.IsActive AS Status, b.CreatedAt
                FROM Boards b
                LEFT JOIN Countries c ON b.CountryId = c.CountryId
                ORDER BY b.CreatedAt DESC
                LIMIT 5;

                SELECT b.BoardId, b.BoardCode, b.BoardName, c.CountryName, b.IsActive AS Status, b.UpdatedAt
                FROM Boards b
                LEFT JOIN Countries c ON b.CountryId = c.CountryId
                WHERE b.UpdatedAt IS NOT NULL
                ORDER BY b.UpdatedAt DESC
                LIMIT 5;
            ";

            using (var multi = await Connection.QueryMultipleAsync(sql))
            {
                var total = await multi.ReadFirstOrDefaultAsync<int>();
                var active = await multi.ReadFirstOrDefaultAsync<int>();
                var inactive = await multi.ReadFirstOrDefaultAsync<int>();
                var byCountry = multi.Read<BoardLookupCountDto>().ToList();
                var byPattern = multi.Read<BoardLookupCountDto>().ToList();
                var byGrading = multi.Read<BoardLookupCountDto>().ToList();
                var recentlyCreated = multi.Read<BoardRecentActivityDto>().ToList();
                var recentlyUpdated = multi.Read<BoardRecentActivityDto>().ToList();

                return new BoardSummaryResponse
                {
                    TotalBoards = total,
                    ActiveBoards = active,
                    InactiveBoards = inactive,
                    BoardsByCountry = byCountry,
                    BoardsByAcademicPattern = byPattern,
                    BoardsByGradingSystem = byGrading,
                    RecentlyCreated = recentlyCreated,
                    RecentlyUpdated = recentlyUpdated
                };
            }
        }

        /// <inheritdoc />
        public async Task<List<Board>> GetBoardsForExportAsync(BoardExportRequest request)
        {
            var validSortFields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "BoardCode", "b.BoardCode" },
                { "BoardName", "b.BoardName" },
                { "CreatedAt", "b.CreatedAt" }
            };

            var orderByColumn = validSortFields.TryGetValue(request.SortBy ?? "", out var col) ? col : "b.BoardName";
            var orderByDirection = string.Equals(request.SortOrder, "DESC", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";

            var sql = @"
                SELECT b.*, c.*, s.*, ap.*, gs.*
                FROM Boards b
                LEFT JOIN Countries c ON b.CountryId = c.CountryId
                LEFT JOIN States s ON b.StateId = s.StateId
                LEFT JOIN AcademicPatterns ap ON b.AcademicPatternId = ap.AcademicPatternId
                LEFT JOIN GradingSystems gs ON b.GradingSystemId = gs.GradingSystemId
                WHERE 1 = 1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                sql += " AND (b.BoardName LIKE @Search OR b.BoardCode LIKE @Search)";
                parameters.Add("Search", $"%{request.Search}%");
            }

            if (!string.IsNullOrWhiteSpace(request.BoardName))
            {
                sql += " AND b.BoardName LIKE @BoardName";
                parameters.Add("BoardName", $"%{request.BoardName}%");
            }

            if (!string.IsNullOrWhiteSpace(request.BoardCode))
            {
                sql += " AND b.BoardCode LIKE @BoardCode";
                parameters.Add("BoardCode", $"%{request.BoardCode}%");
            }

            if (request.CountryId.HasValue)
            {
                sql += " AND b.CountryId = @CountryId";
                parameters.Add("CountryId", request.CountryId.Value);
            }

            if (request.StateId.HasValue)
            {
                sql += " AND b.StateId = @StateId";
                parameters.Add("StateId", request.StateId.Value);
            }

            if (request.AcademicPatternId.HasValue)
            {
                sql += " AND b.AcademicPatternId = @AcademicPatternId";
                parameters.Add("AcademicPatternId", request.AcademicPatternId.Value);
            }

            if (request.GradingSystemId.HasValue)
            {
                sql += " AND b.GradingSystemId = @GradingSystemId";
                parameters.Add("GradingSystemId", request.GradingSystemId.Value);
            }

            if (request.Status.HasValue)
            {
                sql += " AND b.IsActive = @Status";
                parameters.Add("Status", request.Status.Value ? 1 : 0);
            }

            sql += $" ORDER BY {orderByColumn} {orderByDirection};";

            if (Connection.State == ConnectionState.Closed)
            {
                await _context.Database.OpenConnectionAsync();
            }

            var items = await Connection.QueryAsync<Board, Country, State, AcademicPattern, GradingSystem, Board>(
                sql,
                (b, c, s, ap, gs) =>
                {
                    b.Country = c;
                    b.State = s;
                    b.AcademicPattern = ap;
                    b.GradingSystem = gs;
                    return b;
                },
                parameters,
                splitOn: "CountryId,StateId,AcademicPatternId,GradingSystemId");

            return items.ToList();
        }
    }
}
