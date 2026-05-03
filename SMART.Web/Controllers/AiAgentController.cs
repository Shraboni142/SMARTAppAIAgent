using System;
using System.Linq;
using System.Web.Mvc;
using SMART.Web.Services.AI;

namespace SMART.Web.Controllers
{
    [Authorize]
    public class AiAgentController : Controller
    {
        private readonly IAiAgentService _aiAgentService;
        private readonly IAiLlmService _aiLlmService;
        private readonly IErpAiDataService _erpAiDataService;
        private readonly IErpSchemaService _erpSchemaService;
        private readonly IErpSqlGeneratorService _erpSqlGeneratorService;
        private readonly IErpSqlSafetyService _erpSqlSafetyService;
        private readonly IErpQueryExecutorService _erpQueryExecutorService;
        private readonly IErpRelevantTableService _erpRelevantTableService;

        // ✅ NEW ADD: Schema Registry Service
        private readonly IErpSchemaRegistryService _erpSchemaRegistryService;

        public AiAgentController(
            IAiAgentService aiAgentService,
            IAiLlmService aiLlmService,
            IErpAiDataService erpAiDataService,
            IErpSchemaService erpAiDataServiceSchema,
            IErpSqlGeneratorService erpSqlGeneratorService,
            IErpSqlSafetyService erpSqlSafetyService,
            IErpQueryExecutorService erpQueryExecutorService,
            IErpRelevantTableService erpRelevantTableService,
            IErpSchemaRegistryService erpSchemaRegistryService) // ✅ NEW ADD PARAM
        {
            _aiAgentService = aiAgentService;
            _aiLlmService = aiLlmService;
            _erpAiDataService = erpAiDataService;
            _erpSchemaService = erpAiDataServiceSchema;
            _erpSqlGeneratorService = erpSqlGeneratorService;
            _erpSqlSafetyService = erpSqlSafetyService;
            _erpQueryExecutorService = erpQueryExecutorService;
            _erpRelevantTableService = erpRelevantTableService;

            // ✅ NEW ADD ASSIGN
            _erpSchemaRegistryService = erpSchemaRegistryService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SendMessage(int? sessionId, string mode, string message)
        {
            try
            {
                var userId = User.Identity.Name;

                if (string.IsNullOrWhiteSpace(mode))
                    mode = "General";

                if (string.IsNullOrWhiteSpace(message))
                    return Json(new { success = false, message = "Message is required." });

                var session = sessionId.HasValue && sessionId.Value > 0
                    ? _aiAgentService.GetSessionById(sessionId.Value)
                    : null;

                if (session == null)
                {
                    // ✅ This is only chat title, not message limit
                    var title = message.Length > 40 ? message.Substring(0, 40) + "..." : message;
                    session = _aiAgentService.CreateSession(userId, mode, title);
                }

                var setting = _aiAgentService.GetActiveSetting();

                if (mode == "ERP")
                {
                    var employeeListHtml = _erpAiDataService.GetEmployeeListHtml(message);

                    if (!string.IsNullOrEmpty(employeeListHtml))
                    {
                        var saved = _aiAgentService.SaveMessage(session.Id, userId, mode, message, employeeListHtml);

                        return Json(new
                        {
                            success = true,
                            sessionId = saved.SessionId,
                            reply = employeeListHtml,
                            isHtml = true
                        });
                    }

                    var complainListHtml = _erpAiDataService.GetComplainListHtml(message);

                    if (!string.IsNullOrEmpty(complainListHtml))
                    {
                        var saved = _aiAgentService.SaveMessage(session.Id, userId, mode, message, complainListHtml);

                        return Json(new
                        {
                            success = true,
                            sessionId = saved.SessionId,
                            reply = complainListHtml,
                            isHtml = true
                        });
                    }

                    var activeEmployeeListHtml = _erpAiDataService.GetActiveEmployeeListHtml(message);

                    if (!string.IsNullOrEmpty(activeEmployeeListHtml))
                    {
                        var saved = _aiAgentService.SaveMessage(session.Id, userId, mode, message, activeEmployeeListHtml);

                        return Json(new
                        {
                            success = true,
                            sessionId = saved.SessionId,
                            reply = activeEmployeeListHtml,
                            isHtml = true
                        });
                    }

                    // 🔥 Smart DB answer
                    var smartAnswer = _erpAiDataService.GetSmartErpAnswer(message);

                    if (!string.IsNullOrEmpty(smartAnswer))
                    {
                        var saved = _aiAgentService.SaveMessage(
                            session.Id,
                            userId,
                            mode,
                            message,
                            smartAnswer
                        );

                        return Json(new
                        {
                            success = true,
                            sessionId = saved.SessionId,
                            reply = smartAnswer
                        });
                    }

                    // ✅ NEW: Schema based relevant context from DB schema, not manual table list
                    var schemaForSql = _erpSchemaRegistryService.GetRelevantSchemaContext(message);

                    var generatedSql = _erpSqlGeneratorService.GenerateSql(setting, schemaForSql, message);
                    generatedSql = _erpSqlSafetyService.NormalizeSql(generatedSql);

                    if (_erpSqlSafetyService.IsSafeSelectQuery(generatedSql))
                    {
                        var sqlResultHtml = _erpQueryExecutorService.ExecuteSelectQuery(generatedSql);

                        var saved = _aiAgentService.SaveMessage(session.Id, userId, mode, message, sqlResultHtml);

                        return Json(new
                        {
                            success = true,
                            sessionId = saved.SessionId,
                            reply = sqlResultHtml,
                            isHtml = true,
                            generatedSql = generatedSql
                        });
                    }

                    // 🔥 AI fallback with relevant schema context
                    var erpContext = _erpAiDataService.GetErpContext(message);

                    // ✅ NEW: Full schema na, only relevant schema
                    var schemaContext = _erpSchemaRegistryService.GetRelevantSchemaContext(message);

                    message =
                        "You are Smart ERP AI Assistant.\n" +
                        "Use database schema to answer.\n" +
                        "Only generate business answers.\n" +
                        "Do NOT explain code.\n\n" +

                        "DATABASE SCHEMA:\n" + schemaContext + "\n\n" +

                        "ERP DATA CONTEXT:\n" + erpContext + "\n\n" +

                        "USER QUESTION:\n" + message;
                }

                var aiReply = _aiLlmService.AskAi(setting, message);

                _aiAgentService.SaveMessage(session.Id, userId, mode, message, aiReply);

                return Json(new
                {
                    success = true,
                    sessionId = session.Id,
                    reply = aiReply
                });
            }
            catch (Exception ex)
            {
                var error = ex.Message;

                if (ex.InnerException != null)
                    error += " | Inner: " + ex.InnerException.Message;

                if (ex.InnerException != null && ex.InnerException.InnerException != null)
                    error += " | Inner2: " + ex.InnerException.InnerException.Message;

                return Json(new
                {
                    success = false,
                    message = error
                });
            }
        }

        [HttpGet]
        public JsonResult GetChatSessions()
        {
            var userId = User.Identity.Name;

            var data = _aiAgentService.GetUserSessions(userId)
                .OrderByDescending(x => x.Id)
                .Select(x => new
                {
                    id = x.Id,
                    title = x.Title
                })
                .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetChatMessages(int sessionId)
        {
            var messages = _aiAgentService.GetMessagesBySession(sessionId)
                .OrderBy(x => x.Id)
                .Select(x => new
                {
                    userMessage = x.UserMessage,
                    aiReply = x.AiReply
                }).ToList();

            return Json(messages, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSessions()
        {
            var userId = User.Identity.Name;

            var data = _aiAgentService.GetUserSessions(userId)
                .Select(x => new
                {
                    id = x.Id,
                    title = x.Title,
                    mode = x.Mode,
                    createdOn = x.CreatedOn.ToString("dd-MMM-yyyy hh:mm tt")
                })
                .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // ✅ Schema Test - Full DB Schema
        [HttpGet]
        public ContentResult TestSchema()
        {
            var schema = _erpSchemaRegistryService.GetFullSchemaContext();
            return Content(schema, "text/plain");
        }

        // ✅ NEW ADD: Test relevant schema by question
        [HttpGet]
        public ContentResult TestRelevantSchema(string question)
        {
            var schema = _erpSchemaRegistryService.GetRelevantSchemaContext(question);
            return Content(schema, "text/plain");
        }
    }
}