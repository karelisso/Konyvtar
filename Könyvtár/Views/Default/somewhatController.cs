using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Könyvtár.Views.Default
{
    public class somewhatController : Controller
    {
        // GET: somewhat
        public ActionResult Index()
        {
            return View();
        }

        // GET: somewhat/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: somewhat/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: somewhat/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: somewhat/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: somewhat/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: somewhat/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: somewhat/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
