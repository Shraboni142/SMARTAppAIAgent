using System;
using System.Web.Mvc;
using SMART.Web.Models.AI;
using SMART.Web.Services.AI;

namespace SMART.Web.Controllers
{
    [Authorize]
    public class AiAgentAdminController : Controller
    {
        private readonly IAiAgentService _aiAgentService;

        public AiAgentAdminController(IAiAgentService aiAgentService)
        {
            _aiAgentService = aiAgentService;
        }

        public ActionResult Settings()
        {
            var setting = _aiAgentService.GetActiveSetting();

            if (setting == null)
            {
                setting = new AiAgentSetting();
            }

            return View(setting);
        }

        [HttpPost]
        public ActionResult Settings(AiAgentSetting model)
        {
            try
            {
                _aiAgentService.UpdateAiSetting(model);
                TempData["Success"] = "AI settings updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Settings");
        }
    }
}