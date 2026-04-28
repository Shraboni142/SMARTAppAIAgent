using System.Net;
using System.Web.Mvc;
using SMART.Web.Models.HRM;
using SMART.Web.Services.MST;

namespace SMART.Web.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _EmployeeService;

        public EmployeeController(
            IEmployeeService EmployeeService
            )
        {
            _EmployeeService = EmployeeService;
        }

        // GET: Employee
        public ActionResult Index()
        {
            var models = _EmployeeService.GetAllEmployees();
            return View(models);
        }

        // GET: Employee/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = _EmployeeService.GetEmployeeDetails(id ?? 0);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // GET: Employee/Create
        public ActionResult Create()
        {
            return View(new Employee());
        }

        // POST: Employee/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Employee model)
        {
            TempData["msg"] = "Failed! Something went wrong.";

            if (ModelState.IsValid)
            {
                if (_EmployeeService.CheckIfExist(model.Id, model.Code))
                {
                    TempData["msg"] = "Failed! Duplicate code found!";
                    return View(model);
                }

                var abc = _EmployeeService.AddEmployee(model);

                TempData["msg"] = "Success. Employee Saved.";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Employee/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = _EmployeeService.GetEmployeeDetails(id ?? 0);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: Employee/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Employee model)
        {
            TempData["msg"] = "Failed! Something went wrong.";
            if (ModelState.IsValid)
            {
                if (_EmployeeService.CheckIfExist(model.Id, model.Code))
                {
                    TempData["msg"] = "Failed! Duplicate code found!";
                    return View(model);
                }

                var abc = _EmployeeService.UpdateEmployee(model);

                TempData["msg"] = "Success. Employee Updated.";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Employee/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = _EmployeeService.GetEmployeeDetails(id ?? 0);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            var abc = _EmployeeService.DeleteEmployee(id);

            TempData["msg"] = "Success. Employee Deleted.";
            return RedirectToAction("Index");
        }
    }
}
