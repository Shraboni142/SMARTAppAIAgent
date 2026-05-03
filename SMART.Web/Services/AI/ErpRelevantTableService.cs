using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using SMART.Web.Models;

namespace SMART.Web.Services.AI
{
    public interface IErpRelevantTableService
    {
        string GetRelevantSchemaContext(string userQuestion);
    }

    public class ErpRelevantTableService : IErpRelevantTableService
    {
        private readonly ApplicationDbContext _context;

        public ErpRelevantTableService(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GetRelevantSchemaContext(string userQuestion)
        {
            if (string.IsNullOrWhiteSpace(userQuestion))
                return "";

            var keywords = userQuestion.ToLower()
                .Replace("?", "")
                .Replace(",", "")
                .Replace(".", "")
                .Split(' ')
                .Where(x => x.Length > 2)
                .Distinct()
                .ToList();

            var sql = @"
                SELECT 
                    TABLE_NAME,
                    COLUMN_NAME,
                    DATA_TYPE
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = 'dbo'
                ORDER BY TABLE_NAME, ORDINAL_POSITION";

            var allColumns = _context.Database.SqlQuery<SchemaColumnInfo>(sql).ToList();

            var relatedTables = allColumns
                .Where(x =>
                    keywords.Any(k =>
                        x.TABLE_NAME.ToLower().Contains(k) ||
                        x.COLUMN_NAME.ToLower().Contains(k)))
                .Select(x => x.TABLE_NAME)
                .Distinct()
                .Take(5)
                .ToList();

            if (!relatedTables.Any())
            {
                relatedTables = allColumns
                    .Where(x =>
                        x.TABLE_NAME.ToLower().Contains("employee") ||
                        x.TABLE_NAME.ToLower().Contains("complain") ||
                        x.TABLE_NAME.ToLower().Contains("department"))
                    .Select(x => x.TABLE_NAME)
                    .Distinct()
                    .Take(5)
                    .ToList();
            }

            var schemaText = "";

            foreach (var table in relatedTables)
            {
                schemaText += "Table: " + table + "\n";

                var cols = allColumns.Where(x => x.TABLE_NAME == table).ToList();

                foreach (var col in cols)
                {
                    schemaText += "- " + col.COLUMN_NAME + " (" + col.DATA_TYPE + ")\n";
                }

                schemaText += "\n";
            }

            return schemaText;
        }

        private class SchemaColumnInfo
        {
            public string TABLE_NAME { get; set; }
            public string COLUMN_NAME { get; set; }
            public string DATA_TYPE { get; set; }
        }
    }
}