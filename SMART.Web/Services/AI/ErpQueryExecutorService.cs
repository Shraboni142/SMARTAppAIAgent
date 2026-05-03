using System;
using System.Data;
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
            try
            {
                using (var conn = _context.Database.Connection)
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;

                        using (var reader = cmd.ExecuteReader())
                        {
                            var table = new DataTable();
                            table.Load(reader);

                            if (table.Rows.Count == 0)
                                return "No data found.";

                            var html = "<table class='table table-bordered table-striped'>";
                            html += "<thead><tr>";

                            foreach (DataColumn col in table.Columns)
                                html += "<th>" + col.ColumnName + "</th>";

                            html += "</tr></thead><tbody>";

                            foreach (DataRow row in table.Rows)
                            {
                                html += "<tr>";

                                foreach (var item in row.ItemArray)
                                    html += "<td>" + item + "</td>";

                                html += "</tr>";
                            }

                            html += "</tbody></table>";
                            return html;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return "Query execution error: " + ex.Message;
            }
        }
    }
}