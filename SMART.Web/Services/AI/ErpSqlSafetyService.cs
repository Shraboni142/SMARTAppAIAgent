using System;

namespace SMART.Web.Services.AI
{
    public interface IErpSqlSafetyService
    {
        bool IsSafeSelectQuery(string sql);
        string NormalizeSql(string sql);
    }

    public class ErpSqlSafetyService : IErpSqlSafetyService
    {
        public string NormalizeSql(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return "";

            return sql.Replace("```sql", "")
                      .Replace("```", "")
                      .Trim();
        }

        public bool IsSafeSelectQuery(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return false;

            var q = sql.Trim().ToLower();

            if (!q.StartsWith("select"))
                return false;

            string[] blocked =
            {
                " insert ", " update ", " delete ", " drop ", " alter ",
                " truncate ", " create ", " exec ", " execute ", " merge ",
                " grant ", " revoke ", " backup ", " restore "
            };

            foreach (var word in blocked)
            {
                if (q.Contains(word))
                    return false;
            }

            return true;
        }
    }
}