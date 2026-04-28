using System;
using System.Net;
using System.Web.Mvc;
using SMART.Web.Models.INV;
using SMART.Web.Services.INV;

namespace SMART.Web.Controllers
{
    [Authorize]
    public class ItemUnitController : Controller
    {
        private readonly IItemUnitService _ItemUnitService;

        public ItemUnitController(
            IItemUnitService ItemUnitService
            )
        {
            _ItemUnitService = ItemUnitService;
        }

        // GET: ItemUnit
        public ActionResult Index()
        {
            var models = _ItemUnitService.GetAllItemUnits();
            return View(models);
        }

        // GET: ItemUnit/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = _ItemUnitService.GetItemUnitDetails(id ?? 0);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // GET: ItemUnit/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ItemUnit/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ItemUnit model)
        {
            model.IsActive = true;
            model.IsDeleted = false;
            model.CreatedOn = DateTime.Now;

            TempData["msg"] = "Failed! Something went wrong.";

            if (ModelState.IsValid)
            {
                if (_ItemUnitService.CheckIfExist(model.Id, model.UnitName))
                {
                    TempData["msg"] = "Failed! Duplicate name found!";
                    return View(model);
                }

                var abc = _ItemUnitService.AddItemUnit(model);

                TempData["msg"] = "Success. Unit Saved.";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: ItemUnit/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = _ItemUnitService.GetItemUnitDetails(id ?? 0);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: ItemUnit/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ItemUnit model)
        {
            TempData["msg"] = "Failed! Something went wrong.";
            if (ModelState.IsValid)
            {
                if (_ItemUnitService.CheckIfExist(model.Id, model.UnitName))
                {
                    TempData["msg"] = "Failed! Duplicate name found!";
                    return View(model);
                }

                var abc = _ItemUnitService.UpdateItemUnit(model);

                TempData["msg"] = "Success. Unit Updated.";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: ItemUnit/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = _ItemUnitService.GetItemUnitDetails(id ?? 0);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: ItemUnit/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            var abc = _ItemUnitService.DeleteItemUnit(id);

            TempData["msg"] = "Success. Unit Deleted.";
            return RedirectToAction("Index");
        }
    }
}
