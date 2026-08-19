using CollegeManagement.API.Repositories.Interfaces;
using CollegeManagement.API.Models.Reports;
using CollegeManagement.API.Data;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Linq;

namespace CollegeManagement.API.Repositories.Implementations
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly AppDbContext _context;

        public AuditLogRepository(AppDbContext context)
        {
            _context = context;
        }

        private IDbConnection Connection => _context.Database.GetDbConnection();

        public async Task InsertAsync(AuditLog auditLog, IDbTransaction? transaction = null)
        {
            string sql = @"
                INSERT INTO AuditLogs (UserName, Action, EntityName, EntityId, Description, CreatedAt)
                VALUES (@UserName, @Action, @EntityName, @EntityId, @Description, @CreatedAt);";
            
            await Connection.ExecuteAsync(sql, new
            {
                auditLog.UserName,
                auditLog.Action,
                auditLog.EntityName,
                auditLog.EntityId,
                auditLog.Description,
                auditLog.CreatedAt
            }, transaction);
        }

        public async Task<(IEnumerable<AuditLog> auditLogs, int totalCount)> GetHistoryAsync(int entityId, string entityName, int pageNumber, int pageSize)
        {
            int offset = (pageNumber - 1) * pageSize;
            
            string sqlItems = @"
                SELECT AuditLogId, UserName, Action, EntityName, EntityId, Description, CreatedAt
                FROM AuditLogs
                WHERE EntityId = @EntityId AND EntityName = @EntityName
                ORDER BY CreatedAt DESC
                LIMIT @Limit OFFSET @Offset;";

            string sqlCount = @"
                SELECT COUNT(*)
                FROM AuditLogs
                WHERE EntityId = @EntityId AND EntityName = @EntityName;";

            var items = await Connection.QueryAsync<AuditLog>(sqlItems, new
            {
                EntityId = entityId,
                EntityName = entityName,
                Limit = pageSize,
                Offset = offset
            });

            int totalCount = await Connection.ExecuteScalarAsync<int>(sqlCount, new
            {
                EntityId = entityId,
                EntityName = entityName
            });

            return (items, totalCount);
        }
    }
}
