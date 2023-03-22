using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using Antlr.Runtime.Misc;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using Antlr.Runtime.Tree;
using System.Drawing;
using Microsoft.Ajax.Utilities;
using System.Data.Entity.ModelConfiguration.Configuration;

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
            return View("index");
        }
        public ActionResult Main()
        {
            return View("main_page");
        }
        
        public ActionResult Third()
        {
            Session.Clear();
            return View("emptypage");
        }
        public ActionResult MetaPage()
        {
            return View("TheMetaViewer");
        }
        public ActionResult BookPage()
        {
            return View("book_detail");
        }
        public void SetSession(string name, string value)
        {
            Session[name] = null;

            Session[name] = value;
        }
        public string GetSession(string name)
        {

           return Session[name]?.ToString();
        }
        public ActionResult CreateWriter(string name, string name2, string life, string about)
        {
            Writer wm = new Writer();
            wm.writer_Date = life;
            if (name.Length < 1) name = name2;
            if (name.Length < 1 && name2.Length < 1)
            {
                name = "anonimus";
                name2 = "";
            }
            wm.Id = db_book.Writer.Max(q=>q.Id) +1;
            wm.writer_name = name;
            wm.real_name = name2;
            wm.aboutpath = about;
            db_book.Writer.Add(wm);
            db_book.SaveChanges();
            return View("TheMetaViewer");
        }
        public ActionResult CreateCategory(string name)
        {
            Categories cp = new Categories();
            cp.Id = db_book.Categories.Max(q => q.Id) + 1;
            cp.Name = name;
            db_book.Categories.Add(cp);
            db_book.SaveChanges();
            return View("TheMetaViewer");
        }
        public ActionResult CreateAuthor(string name ,string life)
        {
            Author ath = new Author();
            ath.Id = db_book.Author.Max(q => q.Id) + 1;
            ath.name = name;
            ath.Date = life;
            db_book.Author.Add(ath);
            db_book.SaveChanges();
            return View("TheMetaViewer");
        }


        public ActionResult AddBook(string name, string isbn, string auth,string img,string demo)
        {
            konyv kv = new konyv();
            //long? pictureimage;
            if(Request.Files.Count > 0)
            {
                // HttpPostedFile image = Request.Files[0].InputStream;
                using (var binaryReader = new BinaryReader(Request.Files[0].InputStream))
                {
                    var imageData = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
                    Images imageadd = new Images();
                    imageadd.JPG = imageData;
                    db_book.Images.Add(imageadd);
                    db_book.SaveChanges();
                    kv.imageID = db_book.Images.Where(q => q.JPG == imageData).First().Id ;
                }
            }           
            kv.authorId = 0;
            if (auth.Length > 0)
            {
                kv.authorId =  int.Parse( auth);
            }

            kv.ISBN = isbn;
            kv.name = name;
            kv.demo = demo;
            try
            {
                kv.Id = db_book.konyv.Max(q => q.Id) + 1;
            }
            catch (Exception)
            {

                kv.Id = 0;
            }
            db_book.konyv.Add(kv);
            db_book.SaveChanges();
            
            return View("Index");
        }
        public String Load_Image_Base()
        {

            //return "<body>\r\n    <div class=\"container bg-warning\">\r\n        <a href=\"#\">contentált conttent</a> <br>\r\n        <div class=\"alert-danger\">\r\n        </div>\r\n    </div>\r\n     \r\n</body>";
            //return File(db_book.Images.First().JPG, "image/jpg");
            return string.Format(" data:image/png;base64,{0} ", Convert.ToBase64String(db_book.Images.First().JPG));
            //return $"<img src=\"{string.Format("data:image/png;base64,{0}", Convert.ToBase64String(db_book.Images.First().JPG))}\" alt=\"Alternate Text\" style=\"max-width: 100%; max-height: 100%;\" />" ;
            //return byteArrayToImage(db_book.Images.First().JPG);
            //int imageId = Convert.ToInt32(Request.QueryString["id"]);
            //Response.ContentType = "image/*";
            //Response.BinaryWrite((byte[])db_book.Images.First().JPG);
        }
        public ActionResult Load_Image_File()
        {
            byte[] cover = db_book.Images.First().JPG;
            if (cover != null)
            {
                return File(cover, "image/jpg");
            }
            else
            {
                return null;
            }
        }
        public string LoadPage(int id)
        {
            return id + " wodmwoddoq omgw wemogf";        
        }
        public ActionResult Load_Image_File_Id(long id)
        {
            byte[] cover = db_book.Images.Where(q=>q.Id == id).FirstOrDefault().JPG;
            if (cover != null)
            {
                return File(cover, "image/jpg");
            }
            else
            {
                return null;
            }
        }
        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            try
            {
                MemoryStream ms = new MemoryStream(byteArrayIn, 0, byteArrayIn.Length);
                ms.Write(byteArrayIn, 0, byteArrayIn.Length);
                return Image.FromStream(ms, true);//Exception occurs here
            }
            catch { }
            return null;
        }
        public String LiveSearchWriter(string search)
        {

            search = search.Trim();
            if (search.Length < 1)
            {
                return AllBook(search);
            }
            string html_code = "";
            int occurances = 0;




            if (true)
            {
                Dictionary<string, int> irok = new Dictionary<string, int>();
                foreach (var wr in db_book.Writer)
                {
                    foreach (var item in db_book.konyv)
                    {
                        //if (LevenshteinDistance(search.Trim().ToLower(), wr.real_name.ToLower().Trim()) < 10)
                        //{
                        //    html_code += $" <tr class=\"clickable-table\"> <td>{bk.Id} </td>\r\n                    <td>{bk.ISBN}</td>\r\n                    <td> {db_book.Writer.Where(q => q.Id == bk.authorId).FirstOrDefault().writer_name} </td>\r\n                    <td>{bk.name}</td>\r\n                    <td> <img src=\"/Default/Load_Image_File_Id/{bk.imageID}\" alt=\"Alternate Text\" height=\"50px\" /> </td>\r\n                    <td> @bk.Available_Quantity @*<input type=\"number\" min=\"0\" name=\"name\" value=\"@bk.Available_Quantity\" />*@  </td>    \r\n                </tr> ";

                        //}
                        if (wr.real_name.ToLower().Contains(search.ToLower()) || wr.writer_name.ToLower().Contains(search.ToLower()))
                        {
                            if(item.authorId.Equals(wr.Id))
                                //html_code += $" <tr  onclick=\"tbclick(this)\" ondblclick=\"tbdbclick(this)\"> <td>{item.Id} </td>\r\n                    <td>{item.ISBN}</td>\r\n                    <td> {db_book.Writer.Where(q => q.Id == item.authorId).FirstOrDefault().writer_name} </td>\r\n                    <td>{item.name}</td>\r\n                    <td> <img src=\"/Default/Load_Image_File_Id/{item.imageID}\" alt=\"Alternate Text\" height=\"50px\" /> </td>\r\n                    {"<td>" + item.Quantity + "</td>"}    \r\n                </tr> ";
                                html_code += $" <tr  onclick=\"tbclick(this)\" ondblclick=\"tbdbclick(this)\"> <td>{item.Id} </td>\r\n                    <td>{item.ISBN}</td>\r\n                    <td> {db_book.Writer.Where(q => q.Id == item.authorId).FirstOrDefault().writer_name} </td>\r\n                    <td>{item.name}</td>\r\n                          <td>{db_book.Categories.Where(q => q.Id == item.Categories).FirstOrDefault().Name}</td>\r\n                    {"<td>" + item.Quantity + "</td>"}    \r\n                </tr> ";

                        }
                    }
                }
              
                return html_code;
            }
            foreach (var item in db_book.Writer)
            {
                
                if (item.Id.Equals(search.ToLower()))
                {
                    occurances++;
                    //html_code += $"<div>{item.writer_name} </div> <br>";

                }
                else if (item.real_name.ToLower().Contains(search.ToLower()))
                {
                    occurances++;
                    //html_code += $"<div>{item.real_name} </div> <br>";
                }
            }


            if (occurances>0)
            {
                Dictionary<string, int> writers = new Dictionary<string, int>();
                foreach (var item in db_book.Writer)
                {
                    writers.Add(item.writer_name.ToLower(), LevenshteinDistance(search.Trim().ToLower(), item.writer_name.ToLower().Trim()));
                    if (!writers.ContainsKey(item.writer_name.ToLower()))
                    {
                        writers.Add(item.writer_name.ToLower(), LevenshteinDistance(search.Trim().ToLower(), item.real_name.ToLower().Trim()));
                    }
                    else if(LevenshteinDistance(search.Trim().ToLower(), item.real_name.ToLower().Trim()) < writers[item.writer_name.ToLower()])
                    {
                        writers.Add(item.writer_name.ToLower(), LevenshteinDistance(search.Trim().ToLower(), item.real_name.ToLower().Trim()));
                    }
                    if (LevenshteinDistance(search.Trim().ToLower(), item.writer_name.ToLower().Trim()) < 3 + search.Length / 4 + item.writer_name.Length)
                    {
                        occurances++;
                        //html_code += $"<div>{item.writer_name} </div> <br>";
                        foreach (var item2 in db_book.konyv)
                        {
                            if (item2.authorId.Equals(item.Id))
                            {
                                occurances++;
                                html_code += $" <tr class=\"clickable-table\"> <td>{item2.Id} </td>\r\n                    <td>{item2.ISBN}</td>\r\n                    <td> {db_book.Writer.Where(q => q.Id == item2.authorId).FirstOrDefault().writer_name} </td>\r\n                    <td>{item2.name}</td>\r\n                    <td> <img src=\"/Default/Load_Image_File_Id/{item2.imageID}\" alt=\"Alternate Text\" height=\"50px\" /> </td>\r\n                    { ""}    \r\n                </tr> ";

                            }
                        }
                    }
                    else if (LevenshteinDistance(search.ToLower(), item.real_name.ToLower().Trim()) < 3 + search.Length / 4 + item.real_name.Length)
                    {
                        occurances++;
                        //html_code += $"<div>{item.writer_name} </div> <br>";
                        foreach (var item2 in db_book.konyv)
                        {
                            if (item2.authorId.Equals(item.Id))
                            {
                                occurances++;
                                html_code += $" <tr class=\"clickable-table\"> <td>{item2.Id} </td>\r\n                    <td>{item2.ISBN}</td>\r\n                    <td> {db_book.Writer.Where(q => q.Id == item2.authorId).FirstOrDefault().writer_name} </td>\r\n                    <td>{item2.name}</td>\r\n                    <td> <img src=\"/Default/Load_Image_File_Id/{item2.imageID}\" alt=\"Alternate Text\" height=\"50px\" /> </td>\r\n                    { ""}    \r\n                </tr> ";
                            }
                        }
                    }
                }
                writers= writers.OrderBy(q => q.Value).ToDictionary(w => w.Key, w => w.Value);
                int counter = 0;
                foreach (var item in writers)
                {
                    counter++;
                    Writer wm = db_book.Writer.Where(q => q.writer_name == item.Key).FirstOrDefault();
                    //html_code += $"<div>{wm.writer_name} </div> <br>";

                    foreach (var item2 in db_book.konyv)
                    {
                        if (item2.authorId.Equals(wm.Id) || item2.writerID.Equals(wm.Id))
                        {
                            occurances++;
                            //html_code += $" <tr class=\"clickable-table\"> <td>{item2.Id} </td>\r\n                    <td>{item2.ISBN}</td>\r\n                    <td> {db_book.Writer.Where(q => q.Id == item2.authorId).FirstOrDefault().writer_name} </td>\r\n                    <td>{item2.name}</td>\r\n                    <td> <img src=\"/Default/Load_Image_File_Id/{item2.imageID}\" alt=\"Alternate Text\" height=\"50px\" /> </td>\r\n                    <td> @item2.Available_Quantity @*<input type=\"number\" min=\"0\" name=\"name\" value=\"@item2.Available_Quantity\" />*@  </td>    \r\n                </tr> ";
                        }
                    }

                    //if (counter > 1)
                    //{
                    //    break;
                    //}
                }
            }
            return html_code;
        }

        public String LiveSearchName(string search)
        {

            search = search.Trim();
            if (search.Length < 1)
            {
                return AllBook(search);
            }
            string html_code = "";
            int occurances = 0;
            foreach (var item in db_book.konyv)
            {
                if (item.name.ToLower().Contains(search.ToLower()))
                {
                    occurances++;
                    //html_code += $" <tr  onclick=\"tbclick(this)\" ondblclick=\"tbdbclick(this)\"> <td>{ item.Id } </td>\r\n                    <td>{item.ISBN}</td>\r\n                    <td> {db_book.Writer.Where(q => q.Id == item.authorId).FirstOrDefault().writer_name} </td>\r\n                    <td>{item.name}</td>\r\n                    <td> <img src=\"/Default/Load_Image_File_Id/{item.imageID}\" alt=\"Alternate Text\" height=\"50px\" /> </td>\r\n                    { "<td>" + item.Quantity + "</td>" }    \r\n                </tr> ";
                    html_code += $" <tr  onclick=\"tbclick(this)\" ondblclick=\"tbdbclick(this)\"> <td>{item.Id} </td>\r\n                    <td>{item.ISBN}</td>\r\n                    <td> {db_book.Writer.Where(q => q.Id == item.authorId).FirstOrDefault().writer_name} </td>\r\n                    <td>{item.name}</td>\r\n                          <td>{db_book.Categories.Where(q => q.Id == item.Categories).FirstOrDefault().Name}</td>\r\n                    {"<td>" + item.Quantity + "</td>"}    \r\n                </tr> ";
                }
            }
            //if (occurances <= 0)
            //{
            //    Dictionary<string, int> writers = new Dictionary<string, int>();
            //    foreach (var item in db_book.konyv)
            //    {
            //        writers.Add(item.name.ToLower(), LevenshteinDistance(search.Trim().ToLower(), item.name.ToLower().Trim()));
            //        if (!writers.ContainsKey(item.name.ToLower()))
            //        {
            //            writers.Add(item.name.ToLower(), LevenshteinDistance(search.Trim().ToLower(), item.real_name.ToLower().Trim()));
            //        }
            //        else if (LevenshteinDistance(search.Trim().ToLower(), item.real_name.ToLower().Trim()) < writers[item.name.ToLower()])
            //        {
            //            writers.Add(item.name.ToLower(), LevenshteinDistance(search.Trim().ToLower(), item.real_name.ToLower().Trim()));
            //        }
            //        //if (LevenshteinDistance(search.Trim().ToLower(),item.writer_name.ToLower().Trim()) < 3 + search.Length / 4 + item.writer_name.Length)
            //        //{
            //        //    occurances++;
            //        //    html_code += $"<div>{item.writer_name} </div> <br>";
            //        //}
            //        //if (LevenshteinDistance(search.ToLower(), item.real_name.ToLower().Trim()) < 3 + search.Length / 4 + item.real_name.Length)
            //        //{
            //        //    occurances++;
            //        //    html_code += $"<div>{item.writer_name} </div> <br>";
            //        //}
            //    }
            //    writers = writers.OrderBy(q => q.Value).ToDictionary(w => w.Key, w => w.Value);
            //    int counter = 0;
            //    foreach (var item in writers)
            //    {
            //        counter++;
            //        Writer wm = db_book.Writer.Where(q => q.writer_name == item.Key).FirstOrDefault();
            //        html_code += $"<div>{wm.writer_name} </div> <br>";
            //        if (counter > 1)
            //        {
            //            break;
            //        }
            //    }
            //}
            return html_code;
        }

        public String LiveSearchISBN(string search)
        {

            search = search.Trim();
            string html_code = "";
            if (search.Length < 1)
            {
                return AllBook(search);
            }
            foreach (var item in db_book.konyv)
            {
                if (item.ISBN.Contains(search))
                {
                    //html_code += $" <tr  onclick=\"tbclick(this)\" ondblclick=\"tbdbclick(this)\"> <td>{item.Id} </td>\r\n                    <td>{item.ISBN}</td>\r\n                    <td> {db_book.Writer.Where(q => q.Id == item.authorId).FirstOrDefault().writer_name} </td>\r\n                    <td>{item.name}</td>\r\n                    <td> <img src=\"/Default/Load_Image_File_Id/{item.imageID}\" alt=\"Alternate Text\" height=\"50px\" /> </td>\r\n                    {"<td>" + item.Quantity + "</td>"}    \r\n                </tr> ";
                    html_code += $" <tr  onclick=\"tbclick(this)\" ondblclick=\"tbdbclick(this)\"> <td>{item.Id} </td>\r\n                    <td>{item.ISBN}</td>\r\n                    <td> {db_book.Writer.Where(q => q.Id == item.authorId).FirstOrDefault().writer_name} </td>\r\n                    <td>{item.name}</td>\r\n                          <td>{db_book.Categories.Where(q => q.Id == item.Categories).FirstOrDefault().Name}</td>\r\n                    {"<td>" + item.Quantity + "</td>"}    \r\n                </tr> ";
                }
            }
            return html_code;
        }

        private string AllBook(string search)
        {
            string html_code = "";
                foreach (var item in db_book.konyv)
                {
                //html_code += $" <tr  onclick=\"tbclick(this)\" ondblclick=\"tbdbclick(this)\"> <td>{item.Id} </td>\r\n                    <td>{item.ISBN}</td>\r\n                    <td> {db_book.Writer.Where(q => q.Id == item.authorId).FirstOrDefault().writer_name} </td>\r\n                    <td>{item.name}</td>\r\n                    <td> <img src=\"/Default/Load_Image_File_Id/{item.imageID}\" alt=\"Alternate Text\" height=\"50px\" /> </td>\r\n                    {"<td>" + item.Quantity + "</td>"}    \r\n                </tr> ";
                html_code += $" <tr  onclick=\"tbclick(this)\" ondblclick=\"tbdbclick(this)\"> <td>{item.Id} </td>\r\n                    <td>{item.ISBN}</td>\r\n                    <td> {db_book.Writer.Where(q => q.Id == item.authorId).FirstOrDefault().writer_name} </td>\r\n                    <td>{item.name}</td>\r\n                          <td>{db_book.Categories.Where(q => q.Id ==item.Categories).FirstOrDefault().Name}</td>\r\n                    {"<td>" + item.Quantity + "</td>"}    \r\n                </tr> ";
            }
            return html_code;
        }

        static int LevenshteinDistance(string a, string b)
        {
            int[,] distance = new int[a.Length + 1, b.Length + 1];
            for (int i = 0; i <= a.Length; i++)
            {
                distance[i, 0] = i;
            }
            for (int j = 0; j <= b.Length; j++)
            {
                distance[0, j] = j;
            }
            for (int i = 1; i <= a.Length; i++)
            {
                for (int j = 1; j <= b.Length; j++)
                {
                    int cost = (a[i - 1] == b[j - 1]) ? 0 : 1;
                    distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
                }
            }
            return distance[a.Length, b.Length];
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
                return RegisterUser();
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
        public ActionResult RegisterUser()
        {
            return View("Regist");
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
