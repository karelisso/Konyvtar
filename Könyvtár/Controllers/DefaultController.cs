using System;
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
        public static UsersEntities db_user = new UsersEntities();
        public book_vs19Entities1 db_book = new book_vs19Entities1();
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
            return View("index");
        }
        // public Task<ActionResult> CreateUser()
        // {

        // }
        public ActionResult AddBook(string name, string isbn, string auth,string img)
        {
                konyv kv = new konyv();
                kv.authorId = 0;
                if (auth.Length > 0)
                {
                    kv.authorId =  int.Parse( auth);
                }

                kv.ISBN = isbn;
                kv.name = name;
                try
                {
                    kv.Id = db_book.konyv.Max(q => q.Id) + 1;
                }
                catch (Exception)
                {

                    kv.Id = 0;
                }
                //Images img = new Images;
                //img.JPG = img;
                //db_book.Images.Add();
                db_book.konyv.Add(kv);
                db_book.SaveChanges();
                return View("Index");
        }
        public void delbooks(string name) {
            using (book_vs19Entities1 bullshit = new book_vs19Entities1())
            {
                string[] namesplit = name.Split(' ');
                for (int i = 0; i < namesplit.Length; i++)
                {
                    if (namesplit[i].Length <= 0) continue; 
                    int tempid = int.Parse(namesplit[i]);
                    konyv torolj = bullshit.konyv.Where(q => q.Id.Equals(tempid)).FirstOrDefault();
                    bullshit.konyv.Remove(torolj);
                }
                bullshit.SaveChanges();
            }
           // return View("index");
        }

        //public konyv[] Retrievebooks()
        //{
        //    konyv[] kv;
        //    using (book_vs19Entities1 bullshit = new book_vs19Entities1())
        //    {
        //        kv = bullshit.konyv.ToArray();
        //    }
        //    return kv.ToArray();
        //}

        public void Retrievebooks()
        {
            List<String[]> kv = new List<string[]>();
            using (book_vs19Entities1 bullshit = new book_vs19Entities1())
            {
                foreach (var item in bullshit.konyv)
                {         
                    kv.Add(KonyvSTR(item));
                }
               
            }
            System.Diagnostics.Debug.WriteLine(kv[0].ToString());
            Response.Write( kv[0].ToString());
        }
        public ActionResult CreateUser(string Uname, string mail, string Upp, string UppR, bool? Adm)
        {
            if (Upp != UppR)
                return View("Regist");
               
            User_sus account1 = new User_sus();
            user account2 = new user();
            //this should be fine until i find out to use auto increment.
            //account.id = bullshit.user.Count() + 1;
            account1.Username = Uname;
            account2.Username = Uname;
            account1.Userpassword = Upp;
            account1.email = mail;
            account1.phone = "+36-30-555-5555";
            if (Adm ==null)
                Adm = false;
            account2.admin = (bool)Adm ? 1 : 0;
            account2.special_password = Upp;
            //try
            //{
                db_user.User_sus.Add(account1);
                db_user.SaveChanges();

                account2.id = db_user.User_sus.Where(q => q.Username == account1.Username && q.email == account1.email).FirstOrDefault().Id;
                db_book.user.Add(account2);
               
                db_book.SaveChanges();

                //Session["username"] = Uname;
            //}
            //catch (Exception)
            //{
            //   // return View("Regist");
            //}
            
            return View("LogIn");
        }
        public ActionResult LogInUser(string Uname,string Upp,string RegYet)
        {
            if (RegYet != null)
                return View("Regist");
                //Session["username"] = Uname;
                foreach (var item in db_user.User_sus)
                {
                    try
                    {
                        if (item.Username.ToLower() == Uname.ToLower() || item.email.ToLower() == Uname.ToLower())
                        {
                            if (item.Userpassword == Upp)
                            {
                            Session["username"] = Uname;
                            Session["usermail"] = item.email;
                            Session["userid"] = item.Id;
                            return View("Index");                            
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                   
                }
            return View("/error");
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
        public string[] KonyvSTR(konyv inp)
        {
            String[] vm = new string[4] { inp.Id.ToString(), inp.ISBN, inp.name, "author" + db_book.Author.Where(q=>q.Id == inp.authorId).First().name   };
            return vm;
        }
    }
}
