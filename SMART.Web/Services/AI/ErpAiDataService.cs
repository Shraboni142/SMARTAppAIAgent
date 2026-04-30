using System;
using System.Linq;
using SMART.Web.Models;

namespace SMART.Web.Services.AI
{
    public interface IErpAiDataService
    {
        string GetErpContext(string question);
        string GetComplainListHtml(string question);
        string GetEmployeeListHtml(string question);
        string GetSmartErpAnswer(string question);
        string GetActiveEmployeeListHtml(string question);
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
        public string GetComplainListHtml(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
                return null;

            question = question.ToLower();

            if (!(question.Contains("show") || question.Contains("list") || question.Contains("display")))
                return null;

            if (!(question.Contains("complain") || question.Contains("complaint")))
                return null;

            var complains = _context.EmployeeComplains
                .OrderByDescending(x => x.Id)
                .Take(10)
                .ToList();

            if (!complains.Any())
                return "No complain data found.";

            var html = "<table class='table table-bordered table-striped'>";
            html += "<thead><tr>";
            html += "<th>SL</th><th>Employee</th><th>Details</th><th>Status</th>";
            html += "</tr></thead><tbody>";

            int sl = 1;
            foreach (var c in complains)
            {
                html += "<tr>";
                html += "<td>" + sl + "</td>";
                html += "<td>" + c.EmployeeId + "</td>";
                html += "<td>" + (c.OffenceDetails ?? "") + "</td>";
                html += "<td>" + (c.ReviewStatus ?? "") + "</td>";
                html += "</tr>";

                sl++;
            }

            html += "</tbody></table>";

            return html;
        }
        public string GetEmployeeListHtml(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
                return null;

            question = question.ToLower();

            if (!(question.Contains("show") || question.Contains("list") || question.Contains("display")))
                return null;

            if (!question.Contains("employee"))
                return null;

            var employees = _context.Employees
                .OrderBy(x => x.Id)
                .Take(10)
                .ToList();

            if (!employees.Any())
                return "No employee data found.";

            var html = "<table class='table table-bordered table-striped'>";
            html += "<thead><tr>";
            html += "<th>SL</th><th>Employee Id</th><th>Name</th><th>Active</th>";
            html += "</tr></thead><tbody>";

            int sl = 1;
            foreach (var emp in employees)
            {
                var name = emp.Name ?? "";

                html += "<tr>";
                html += "<td>" + sl + "</td>";
                html += "<td>" + emp.Code + "</td>";
                html += "<td>" + name + "</td>";
                html += "<td>" + emp.IsActive + "</td>";
                html += "</tr>";

                sl++;
            }

            html += "</tbody></table>";

            return html;
        }
        public string GetActiveEmployeeListHtml(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
                return null;

            question = question.ToLower();

            if (!question.Contains("active") || !question.Contains("employee"))
                return null;

            var employees = _context.Employees
                .Where(x => x.IsActive == true)
                .OrderBy(x => x.Id)
                .Take(10)
                .ToList();

            if (!employees.Any())
                return "No active employee data found.";

            var html = "<table class='table table-bordered table-striped'>";
            html += "<thead><tr>";
            html += "<th>SL</th><th>Code</th><th>Name</th><th>Active</th>";
            html += "</tr></thead><tbody>";

            int sl = 1;
            foreach (var emp in employees)
            {
                html += "<tr>";
                html += "<td>" + sl + "</td>";
                html += "<td>" + emp.Code + "</td>";
                html += "<td>" + emp.Name + "</td>";
                html += "<td>" + emp.IsActive + "</td>";
                html += "</tr>";
                sl++;
            }

            html += "</tbody></table>";
            return html;
        }
        public string GetSmartErpAnswer(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
                return null;

            question = question.ToLower();

            try
            {
                // Employee Count
                if (question.Contains("how many") && question.Contains("employee"))
                {
                    var count = _context.Employees.Count();
                    return "Total employees in the system: " + count;
                }

                // Employee Count
                if (
                    (question.Contains("how many") || question.Contains("total") || question.Contains("count")) &&
                    question.Contains("employee")
                )
                {
                    var count = _context.Employees.Count();
                    return "Total employees in the system: " + count;
                }

                // Active Employees
                if (question.Contains("active employee"))
                {
                    var count = _context.Employees.Count(x => x.IsActive == true);
                    return "Total active employees: " + count;
                }

                return null;
            }
            catch (Exception ex)
            {
                return "ERP Smart Query Error: " + ex.Message;
            }
        }
    }
}