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

        public AiAgentController(
    IAiAgentService aiAgentService,
    IAiLlmService aiLlmService,
    IErpAiDataService erpAiDataService)
        {
            _aiAgentService = aiAgentService;
            _aiLlmService = aiLlmService;
            _erpAiDataService = erpAiDataService;
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
                    var title = message.Length > 40 ? message.Substring(0, 40) + "..." : message;
                    session = _aiAgentService.CreateSession(userId, mode, title);
                }

                var setting = _aiAgentService.GetActiveSetting();

                if (mode == "ERP")
                {
                    var erpContext = _erpAiDataService.GetErpContext(message);

                    message =
                        "ERP Assistant Query: " + message +
                        "\n\nUse this ERP database context:\n" + erpContext;
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
                    error += " | Inner 2: " + ex.InnerException.InnerException.Message;

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
    }
}