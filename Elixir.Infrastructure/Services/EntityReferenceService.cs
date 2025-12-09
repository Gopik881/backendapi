using Elixir.Application.Interfaces.Services;
using Elixir.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;


namespace Elixir.Infrastructure.Persistance.Services;
//public class EntityReferenceService : IEntityReferenceService
//{
//    private readonly ElixirHRDbContext _dbContext;

//    public EntityReferenceService(ElixirHRDbContext dbContext)
//    {
//        _dbContext = dbContext;
//    }

//    public async Task<bool> HasActiveReferencesAsync(string columnName, int entityId)
//    {
//        var tables = await _dbContext.Database.SqlQueryRaw<string>(
//            $"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WHERE COLUMN_NAME = @columnName",
//            new SqlParameter("@columnName", columnName)
//        ).ToListAsync();

//        foreach (var table in tables)
//        {
//            var sql = $"SELECT 1  FROM {table} WHERE {columnName} = @entityId AND IsDeleted = 0";
//            var exists = await _dbContext.Database.SqlQueryRaw<int>(sql, new SqlParameter("@entityId", entityId)).ToListAsync();
//            if (exists.Count>0)
//                return true;
//        }
//        return false;
//    }
//    public async Task<int?> GetActiveReferenceIdAsync(string columnName, int entityId)
//    {
//        var tables = await _dbContext.Database.SqlQueryRaw<string>(
//            $"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WHERE COLUMN_NAME = @columnName",
//            new SqlParameter("@columnName", columnName)
//        ).ToListAsync();

//        foreach (var table in tables)
//        {
//            var sql = $"SELECT Id FROM {table} WHERE {columnName} = @entityId AND IsDeleted = 0";
//            var ids = await _dbContext.Database.SqlQueryRaw<int>(sql, new SqlParameter("@entityId", entityId)).ToListAsync();
//            if (ids.Count > 0)
//                return ids[0];
//        }
//        return null;
//    }

//}

public class EntityReferenceService : IEntityReferenceService
{
    private readonly ElixirHRDbContext _dbContext;

    public EntityReferenceService(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> HasActiveReferencesAsync(string columnName, int entityId)
    {
        // Get referencing tables with schema
        var tables = await _dbContext.Database.SqlQueryRaw<string>(
            "SELECT TABLE_SCHEMA + '.' + TABLE_NAME FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WHERE COLUMN_NAME = @columnName",
            new SqlParameter("@columnName", columnName)
        ).ToListAsync();

        foreach (var fullName in tables)
        {
            // split schema and table
            var parts = fullName.Split('.', 2);
            var schema = parts.Length == 2 ? parts[0] : "dbo";
            var table = parts.Length == 2 ? parts[1] : parts[0];

            // Get data type of the referenced column
            var colTypeList = await _dbContext.Database.SqlQueryRaw<string>(
                "SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @table AND COLUMN_NAME = @columnName",
                new SqlParameter("@schema", schema),
                new SqlParameter("@table", table),
                new SqlParameter("@columnName", columnName)
            ).ToListAsync();

            if (colTypeList.Count == 0)
                continue;

            var dataType = colTypeList[0];
            if (!IsIntegerType(dataType))
                continue; // skip tables where the referenced column isn't integer-compatible

            // Check if IsDeleted exists on table
            var isDeletedCountList = await _dbContext.Database.SqlQueryRaw<int>(
                "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @table AND COLUMN_NAME = 'IsDeleted'",
                new SqlParameter("@schema", schema),
                new SqlParameter("@table", table)
            ).ToListAsync();

            var hasIsDeleted = isDeletedCountList.Count > 0 && isDeletedCountList[0] > 0;

            // Build safe SQL (bracket identifiers)
            var sql = $"SELECT TOP(1) 1 FROM [{schema}].[{table}] WHERE [{columnName}] = @entityId";
            if (hasIsDeleted)
                sql += " AND [IsDeleted] = 0";

            var exists = await _dbContext.Database.SqlQueryRaw<int>(sql, new SqlParameter("@entityId", entityId)).ToListAsync();
            if (exists.Count > 0)
                return true;
        }

        return false;
    }

    public async Task<int?> GetActiveReferenceIdAsync(string columnName, int entityId)
    {
        // Get referencing tables with schema
        var tables = await _dbContext.Database.SqlQueryRaw<string>(
            "SELECT TABLE_SCHEMA + '.' + TABLE_NAME FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WHERE COLUMN_NAME = @columnName",
            new SqlParameter("@columnName", columnName)
        ).ToListAsync();

        foreach (var fullName in tables)
        {
            // split schema and table
            var parts = fullName.Split('.', 2);
            var schema = parts.Length == 2 ? parts[0] : "dbo";
            var table = parts.Length == 2 ? parts[1] : parts[0];

            // Get data type of the referenced column
            var colTypeList = await _dbContext.Database.SqlQueryRaw<string>(
                "SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @table AND COLUMN_NAME = @columnName",
                new SqlParameter("@schema", schema),
                new SqlParameter("@table", table),
                new SqlParameter("@columnName", columnName)
            ).ToListAsync();

            if (colTypeList.Count == 0)
                continue;

            var dataType = colTypeList[0];
            if (!IsIntegerType(dataType))
                continue; // skip tables where the referenced column isn't integer-compatible

            // Ensure the Id column is integer-compatible too
            var idTypeList = await _dbContext.Database.SqlQueryRaw<string>(
                "SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @table AND COLUMN_NAME = 'Id'",
                new SqlParameter("@schema", schema),
                new SqlParameter("@table", table)
            ).ToListAsync();

            if (idTypeList.Count == 0)
                continue;

            var idType = idTypeList[0];
            if (!IsIntegerType(idType))
                continue; // skip if Id is not integer-compatible (avoid uniqueidentifier -> int clash)

            // Check if IsDeleted exists on table
            var isDeletedCountList = await _dbContext.Database.SqlQueryRaw<int>(
                "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @table AND COLUMN_NAME = 'IsDeleted'",
                new SqlParameter("@schema", schema),
                new SqlParameter("@table", table)
            ).ToListAsync();

            var hasIsDeleted = isDeletedCountList.Count > 0 && isDeletedCountList[0] > 0;

            // Build safe SQL (bracket identifiers)
            var sql = $"SELECT TOP(1) [Id] FROM [{schema}].[{table}] WHERE [{columnName}] = @entityId";
            if (hasIsDeleted)
                sql += " AND [IsDeleted] = 0";

            var ids = await _dbContext.Database.SqlQueryRaw<int>(sql, new SqlParameter("@entityId", entityId)).ToListAsync();
            if (ids.Count > 0)
                return ids[0];
        }

        return null;
    }

    private static bool IsIntegerType(string dataType)
    {
        if (string.IsNullOrWhiteSpace(dataType))
            return false;

        switch (dataType.Trim().ToLowerInvariant())
        {
            case "int":
            case "bigint":
            case "smallint":
            case "tinyint":
                return true;
            default:
                return false;
        }
    }
}
