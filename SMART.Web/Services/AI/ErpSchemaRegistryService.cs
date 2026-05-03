using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using SMART.Web.Models;

namespace SMART.Web.Services.AI
{
    public interface IErpSchemaRegistryService
    {
        string GetFullSchemaContext();
        string GetRelevantSchemaContext(string userQuestion, int maxTables = 7);
        List<string> GetRelevantTableNames(string userQuestion, int maxTables = 7);
    }

    public class ErpSchemaRegistryService : IErpSchemaRegistryService
    {
        private readonly ApplicationDbContext _context;

        public ErpSchemaRegistryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GetFullSchemaContext()
        {
            var allColumns = LoadSchemaColumns();

            var sb = new StringBuilder();

            foreach (var tableGroup in allColumns.GroupBy(x => x.FullTableName))
            {
                sb.AppendLine("Table: " + tableGroup.Key);

                foreach (var col in tableGroup.OrderBy(x => x.ORDINAL_POSITION))
                {
                    sb.AppendLine("- " + col.COLUMN_NAME + " (" + col.DATA_TYPE + ")");
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        public string GetRelevantSchemaContext(string userQuestion, int maxTables = 7)
        {
            var allColumns = LoadSchemaColumns();
            var relatedTables = GetRelevantTableNames(userQuestion, maxTables);

            if (!relatedTables.Any())
                return "";

            var sb = new StringBuilder();

            foreach (var table in relatedTables)
            {
                var cols = allColumns
                    .Where(x => x.FullTableName.Equals(table, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(x => x.ORDINAL_POSITION)
                    .ToList();

                sb.AppendLine("Table: " + table);

                foreach (var col in cols)
                {
                    sb.AppendLine("- " + col.COLUMN_NAME + " (" + col.DATA_TYPE + ")");
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        public List<string> GetRelevantTableNames(string userQuestion, int maxTables = 7)
        {
            var allColumns = LoadSchemaColumns();

            if (string.IsNullOrWhiteSpace(userQuestion))
                return new List<string>();

            var keywords = userQuestion.ToLower()
                .Replace("?", " ")
                .Replace(",", " ")
                .Replace(".", " ")
                .Replace("_", " ")
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => x.Length > 2)
                .Distinct()
                .ToList();

            AddSynonymKeywords(keywords, userQuestion);

            var tableScores = allColumns
                .GroupBy(x => x.FullTableName)
                .Select(g => new
                {
                    TableName = g.Key,
                    Score = GetTableScore(g.Key, g.ToList(), keywords)
                })
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .ThenBy(x => x.TableName)
                .Take(maxTables)
                .Select(x => x.TableName)
                .ToList();

            return tableScores;
        }

        private int GetTableScore(string tableName, List<SchemaColumnInfo> columns, List<string> keywords)
        {
            int score = 0;
            var lowerTable = tableName.ToLower();

            foreach (var key in keywords)
            {
                if (lowerTable.Contains(key))
                    score += 10;

                foreach (var col in columns)
                {
                    var lowerCol = col.COLUMN_NAME.ToLower();

                    if (lowerCol.Contains(key))
                        score += 3;
                }
            }

            return score;
        }

        private void AddSynonymKeywords(List<string> keywords, string userQuestion)
        {
            var q = userQuestion.ToLower();

            if (q.Contains("employee") || q.Contains("staff") || q.Contains("officer"))
            {
                AddIfMissing(keywords, "employee");
                AddIfMissing(keywords, "emp");
                AddIfMissing(keywords, "hrm");
            }

            if (q.Contains("complain") || q.Contains("complaint") || q.Contains("grievance"))
            {
                AddIfMissing(keywords, "complain");
                AddIfMissing(keywords, "complaint");
                AddIfMissing(keywords, "offence");
                AddIfMissing(keywords, "status");
            }

            if (q.Contains("department") || q.Contains("dept"))
            {
                AddIfMissing(keywords, "department");
                AddIfMissing(keywords, "dept");
            }

            if (q.Contains("company") || q.Contains("organization"))
            {
                AddIfMissing(keywords, "company");
                AddIfMissing(keywords, "mst");
            }

            if (q.Contains("item") || q.Contains("unit") || q.Contains("inventory"))
            {
                AddIfMissing(keywords, "item");
                AddIfMissing(keywords, "unit");
                AddIfMissing(keywords, "inv");
            }
        }

        private void AddIfMissing(List<string> keywords, string value)
        {
            if (!keywords.Contains(value))
                keywords.Add(value);
        }

        private List<SchemaColumnInfo> LoadSchemaColumns()
        {
            var sql = @"
                SELECT
                    TABLE_SCHEMA,
                    TABLE_NAME,
                    COLUMN_NAME,
                    DATA_TYPE,
                    ORDINAL_POSITION
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA NOT IN ('sys', 'INFORMATION_SCHEMA')
                  AND TABLE_NAME NOT LIKE 'AspNet%'
                  AND TABLE_NAME NOT LIKE '__MigrationHistory%'
                ORDER BY TABLE_SCHEMA, TABLE_NAME, ORDINAL_POSITION";

            return _context.Database.SqlQuery<SchemaColumnInfo>(sql).ToList();
        }

        private class SchemaColumnInfo
        {
            public string TABLE_SCHEMA { get; set; }
            public string TABLE_NAME { get; set; }
            public string COLUMN_NAME { get; set; }
            public string DATA_TYPE { get; set; }
            public int ORDINAL_POSITION { get; set; }

            public string FullTableName
            {
                get { return "[" + TABLE_SCHEMA + "].[" + TABLE_NAME + "]"; }
            }
        }
    }
}