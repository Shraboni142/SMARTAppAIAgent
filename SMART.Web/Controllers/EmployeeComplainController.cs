using SMART.Web.Models.HRM;
using SMART.Web.Services.HRM;
using System;
using System.Net;
using System.Web.Mvc;

namespace SMART.Web.Controllers
{
    [Authorize]
    public class EmployeeComplainController : Controller
    {
        private readonly IEmployeeComplainService _employeeComplainService;

        public EmployeeComplainController(IEmployeeComplainService employeeComplainService)
        {
            _employeeComplainService = employeeComplainService;
        }

        // GET: EmployeeComplain
        public ActionResult Index()
        {
            ViewBag.Title = "Complain List";
            ViewBag.IsReviewMode = false;
            ViewBag.ActiveMenu = "ComplainList";

            var models = _employeeComplainService.GetAllEmployeeComplains();
            return View(models);
        }
        // GET: EmployeeComplain/Review
        public ActionResult Review()
        {
            ViewBag.Title = "Employee Complain Review";
            ViewBag.IsReviewMode = true;
            ViewBag.ActiveMenu = "EmployeeComplainReview";

            var models = _employeeComplainService.GetAllEmployeeComplains();
            return View("Index", models);
        }

        // GET: EmployeeComplain/MiddleManList
        public ActionResult MiddleManList()
        {
            ViewBag.Title = "Complain List";
            ViewBag.IsReviewMode = false;
            ViewBag.IsMiddleManMode = true;
            ViewBag.ActiveMenu = "ComplainList";

            var models = _employeeComplainService.GetAllEmployeeComplains();
            return View("Index", models);
        }

        // GET: EmployeeComplain/MiddleManView/5
        public ActionResult MiddleManView(int? id)
        {
            ViewBag.Title = "Details";
            ViewBag.IsMiddleManMode = true;
            ViewBag.ActiveMenu = "MiddleManComplainList";

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var model = _employeeComplainService.GetEmployeeComplainDetails(id ?? 0);
            if (model == null)
            {
                return HttpNotFound();
            }

            return View("ViewComplain", model);
        
        }
        // GET: EmployeeComplain/Create
        public ActionResult Create()
        {
            ViewBag.Title = "Add Complain Action Information";
            ViewBag.ActiveMenu = "ComplainList";
            return View(new EmployeeComplain());
        }

        // POST: EmployeeComplain/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmployeeComplain model)
        {
            TempData["msg"] = "Failed! Something went wrong.";

            if (ModelState.IsValid)
            {
                var result = _employeeComplainService.AddEmployeeComplain(model, Server);

                if (result)
                {
                    TempData["msg"] = "Success. Complain action saved.";
                    return RedirectToAction("Index");
                }
            }

            return View(model);
        }

        // GET: EmployeeComplain/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.Title = "Edit Complain Action Information";
            ViewBag.ActiveMenu = "ComplainList";

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var model = _employeeComplainService.GetEmployeeComplainDetails(id ?? 0);
            if (model == null)
            {
                return HttpNotFound();
            }

            return View("Create", model);
        }

        // POST: EmployeeComplain/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EmployeeComplain model)
        {
            TempData["msg"] = "Failed! Something went wrong.";

            if (ModelState.IsValid)
            {
                var result = _employeeComplainService.UpdateEmployeeComplain(model, Server);

                if (result)
                {
                    TempData["msg"] = "Success. Complain action updated.";
                    return RedirectToAction("Index");
                }
            }

            ViewBag.Title = "Edit Complain Action Information";
            return View("Create", model);
        }

        // POST: EmployeeComplain/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var result = _employeeComplainService.DeleteEmployeeComplain(id);

            TempData["msg"] = result
                ? "Success. Complain action deleted."
                : "Failed! Unable to delete complain action.";

            return RedirectToAction("Index");
        }

        // GET: EmployeeComplain/ViewComplain/5
        public ActionResult ViewComplain(int? id)
        {
            ViewBag.Title = "View Complain Action Information";

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var model = _employeeComplainService.GetEmployeeComplainDetails(id ?? 0);
            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        // POST: EmployeeComplain/UpdateReviewStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateReviewStatus(int id, string reviewStatus)
        {
            if (id <= 0 || string.IsNullOrWhiteSpace(reviewStatus))
            {
                TempData["msg"] = "Failed! Invalid review status update request.";
                return RedirectToAction("Review");
            }

            var result = _employeeComplainService.UpdateReviewStatus(id, reviewStatus);

            TempData["msg"] = result
                ? "Success. Review status updated."
                : "Failed! Unable to update review status.";

            return RedirectToAction("ViewComplain", new { id = id });
        }
    }
}