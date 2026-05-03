using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using SMART.Web.Models;

namespace SMART.Web.Services.AI
{
    public interface IErpQueryExecutorService
    {
        string ExecuteSelectQuery(string sql);
    }

    public class ErpQueryExecutorService : IErpQueryExecutorService
    {
        private readonly ApplicationDbContext _context;

        public ErpQueryExecutorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public string ExecuteSelectQuery(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return "No valid SQL query generated.";

            var connection = _context.Database.Connection;

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandTimeout = 60;

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    var sb = new StringBuilder();

                    sb.Append("<div class='table-responsive'>");
                    sb.Append("<table class='table table-bordered table-striped table-sm'>");

                    sb.Append("<thead><tr>");
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        sb.Append("<th>");
                        sb.Append(HttpUtility.HtmlEncode(reader.GetName(i)));
                        sb.Append("</th>");
                    }
                    sb.Append("</tr></thead>");

                    sb.Append("<tbody>");

                    int rowCount = 0;

                    while (reader.Read())
                    {
                        rowCount++;

                        sb.Append("<tr>");

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var value = reader.IsDBNull(i) ? "" : Convert.ToString(reader.GetValue(i));

                            sb.Append("<td>");
                            sb.Append(HttpUtility.HtmlEncode(value));
                            sb.Append("</td>");
                        }

                        sb.Append("</tr>");
                    }

                    if (rowCount == 0)
                    {
                        sb.Append("<tr><td colspan='");
                        sb.Append(reader.FieldCount);
                        sb.Append("'>No data found.</td></tr>");
                    }

                    sb.Append("</tbody>");
                    sb.Append("</table>");
                    sb.Append("</div>");

                    return sb.ToString();
                }
            }
        }
    }
}