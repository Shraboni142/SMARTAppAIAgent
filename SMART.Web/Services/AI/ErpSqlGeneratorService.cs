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
            var prompt =
                "You are a SQL Server query generator for Smart ERP.\n" +
                "Generate ONLY one safe read-only SELECT query.\n" +
                "Do not explain anything.\n" +
                "Do not use INSERT, UPDATE, DELETE, DROP, ALTER, EXEC.\n" +
                "Use TOP 20 if result may return multiple rows.\n\n" +
                "DATABASE SCHEMA:\n" + schemaContext + "\n\n" +
                "USER QUESTION:\n" + userQuestion + "\n\n" +
                "SQL:";

            return _aiLlmService.AskAi(setting, prompt);
        }
    }
}