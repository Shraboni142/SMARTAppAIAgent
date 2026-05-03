using SMART.Web.Models.AI;

namespace SMART.Web.Services.AI
{
    public interface IErpSqlGeneratorService
    {
        string GenerateSql(AiAgentSetting setting, string schemaContext, string userQuestion);
    }

    public class ErpSqlGeneratorService : IErpSqlGeneratorService
    {
        private readonly IAiLlmService _aiLlmService;

        public ErpSqlGeneratorService(IAiLlmService aiLlmService)
        {
            _aiLlmService = aiLlmService;
        }

        public string GenerateSql(AiAgentSetting setting, string schemaContext, string userQuestion)
        {
            if (string.IsNullOrWhiteSpace(schemaContext))
                return "";

            var prompt =
                "You are a SQL Server query generator for Smart ERP.\n" +
                "Return ONLY one SQL SELECT query. No explanation. No markdown.\n\n" +

                "STRICT RULES:\n" +
                "1. Use only SELECT query.\n" +
                "2. Use only tables and columns from DATABASE SCHEMA.\n" +
                "3. Table name must always be schema-qualified with square brackets, example: [hrm].[EmployeeComplains].\n" +
                "4. Always create table alias and use alias for columns.\n" +
                "5. Never write column like hrm.EmployeeComplains.EmployeeId.\n" +
                "6. Correct column usage example: EC.EmployeeId, EC.EmployeeName, EC.ReviewStatus.\n" +
                "7. Correct table usage example: FROM [hrm].[EmployeeComplains] EC.\n" +
                "8. Do not use INSERT, UPDATE, DELETE, DROP, ALTER, CREATE, TRUNCATE, MERGE, EXEC.\n" +
                "9. Always add TOP 20 for list/detail query.\n" +
                "10. For count question, use COUNT(*) only.\n" +
                "11. If IsDeleted column exists, add condition: ISNULL(alias.IsDeleted, 0) = 0.\n" +
                "12. If IsActive column exists and user asks active data, add condition: alias.IsActive = 1.\n\n" +

                "DATABASE SCHEMA:\n" + schemaContext + "\n\n" +

                "EXAMPLE FOR COMPLAIN LIST:\n" +
                "SELECT TOP 20 EC.EmployeeId, EC.EmployeeCode, EC.EmployeeName, EC.ReviewStatus, EC.OffenceType, EC.DateOfNotice FROM [hrm].[EmployeeComplains] EC WHERE ISNULL(EC.IsDeleted, 0) = 0\n\n" +

                "EXAMPLE FOR EMPLOYEE LIST:\n" +
                "SELECT TOP 20 E.Id, E.Code, E.Name, E.IsActive FROM [hrm].[Employees] E WHERE ISNULL(E.IsDeleted, 0) = 0\n\n" +

                "USER QUESTION:\n" + userQuestion + "\n\n" +
                "SQL:";

            return _aiLlmService.AskAi(setting, prompt);
        }
    }
}