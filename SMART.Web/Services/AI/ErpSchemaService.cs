using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using SMART.Web.Models;

namespace SMART.Web.Services.AI
{
    public interface IErpSchemaService
    {
        string GetSchemaContext();
    }

    public class ErpSchemaService : IErpSchemaService
    {
        private readonly ApplicationDbContext _context;

        public ErpSchemaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GetSchemaContext()
        {
            try
            {
                var sql = @"
                    SELECT 
                        TABLE_NAME,
                        COLUMN_NAME,
                        DATA_TYPE
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = 'dbo'
                    ORDER BY TABLE_NAME, ORDINAL_POSITION";

                var result = _context.Database.SqlQuery<SchemaColumnInfo>(sql).ToList();

                var schemaText = "";

                foreach (var tableGroup in result.GroupBy(x => x.TABLE_NAME))
                {
                    schemaText += "Table: " + tableGroup.Key + "\n";

                    foreach (var col in tableGroup)
                    {
                        schemaText += "- " + col.COLUMN_NAME + " (" + col.DATA_TYPE + ")\n";
                    }

                    schemaText += "\n";
                }

                return schemaText;
            }
            catch (Exception ex)
            {
                return "Schema read error: " + ex.Message;
            }
        }

        private class SchemaColumnInfo
        {
            public string TABLE_NAME { get; set; }
            public string COLUMN_NAME { get; set; }
            public string DATA_TYPE { get; set; }
        }
    }
}