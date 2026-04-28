using System;
using System.Linq;
using SMART.Web.Models;

namespace SMART.Web.Services.AI
{
    public interface IErpAiDataService
    {
        string GetErpContext(string question);
    }

    public class ErpAiDataService : IErpAiDataService
    {
        private readonly ApplicationDbContext _context;

        public ErpAiDataService(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GetErpContext(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
                return "";

            question = question.ToLower();

            try
            {
                if (question.Contains("employee"))
                {
                    var totalEmployee = _context.Employees.Count();

                    return "ERP Data Context: Total employee count is " + totalEmployee + ".";
                }

                if (question.Contains("complain") || question.Contains("complaint"))
                {
                    var totalComplain = _context.EmployeeComplains.Count();

                    return "ERP Data Context: Total employee complain count is " + totalComplain + ".";
                }

                return "ERP Data Context: No specific ERP module data matched for this question.";
            }
            catch (Exception ex)
            {
                return "ERP Data Context Error: " + ex.Message;
            }
        }
    }
}