﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using Antlr.Runtime.Misc;
using System.Threading.Tasks;

namespace Könyvtár.App_Data
{
    public class DefaultController : Controller
    {
        // GET: Default
        public ActionResult Index()
        {
            Random rng = new Random();
            //using (book_vs19Entities bullshit = new book_vs19Entities())
            //{
            //    int idd = rng.Next();
                
            //    konyv teszt = new konyv();
            //    teszt.author = 666;
            //    teszt.ISBN = "ISBN001";
            //    teszt.name = "egy könny";
            //    teszt.Id = idd;
            //    bullshit.konyv.Add(teszt);
            //    if (bullshit.konyv.Where(q => q.Id == teszt.Id).Count() == 0)
            //    {
            //        bullshit.SaveChanges();
            //    }

            //}
            return View();
        }
        //public Task<ActionResult> CreateUser()
        //{

        //}
        public ActionResult CreateUser(string Uname, string Upp)
        {
            using (book_vs19Entities bullshit = new book_vs19Entities())
            {
                user account = new user();
                account.Username = Uname;
                account.Userpeeword = Upp; 
                bullshit.user.Add(account);
                bullshit.SaveChanges();
            }
            return View();
        }
        public ActionResult Secnd()
        {
            return View("LogIn");
        }
        // GET: Default/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Default/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Default/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("secondary");
            }
            catch
            {
                return View();
            }
        }

        // GET: Default/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Default/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("secondary");
            }
            catch
            {
                return View();
            }
        }

        // GET: Default/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Default/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("secondary");
            }
            catch
            {
                return View();
            }
        }
    }
}
