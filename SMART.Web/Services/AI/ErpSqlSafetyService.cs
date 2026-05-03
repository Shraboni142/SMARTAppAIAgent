using System;
using System.Text.RegularExpressions;

namespace SMART.Web.Services.AI
{
    public interface IErpSqlSafetyService
    {
        string NormalizeSql(string sql);
        bool IsSafeSelectQuery(string sql);
    }

    public class ErpSqlSafetyService : IErpSqlSafetyService
    {
        public string NormalizeSql(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return "";

            sql = sql.Trim();

            sql = sql.Replace("```sql", "");
            sql = sql.Replace("```", "");
            sql = sql.Trim();

            if (sql.EndsWith(";"))
                sql = sql.Substring(0, sql.Length - 1).Trim();

            return sql;
        }

        public bool IsSafeSelectQuery(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return false;

            var q = sql.Trim();
            var lower = q.ToLower();

            if (!lower.StartsWith("select"))
                return false;

            if (lower.Contains(";"))
                return false;

            if (Regex.IsMatch(lower, @"\b(insert|update|delete|drop|alter|create|truncate|merge|exec|execute|grant|revoke|backup|restore)\b"))
                return false;

            if (Regex.IsMatch(lower, @"\b(sys\.|information_schema|xp_|sp_)\b"))
                return false;

            if (lower.Contains("--") || lower.Contains("/*") || lower.Contains("*/"))
                return false;

            if (!Regex.IsMatch(lower, @"\btop\s+\(?\d+\)?\b") && !Regex.IsMatch(lower, @"\bcount\s*\("))
                return false;

            if (!Regex.IsMatch(q, @"\[[^\]]+\]\.\[[^\]]+\]"))
                return false;

            return true;
        }
    }
}