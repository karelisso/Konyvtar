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
        public ActionResult TagPage()
        {
            return View("TagAdd");
        }
        public ActionResult Main()
        {
            return View("main_page");
        }
        
        public ActionResult ReaderCard()
        {
            return View("reader_card");
        }

        public ActionResult start()
        {
            if (Session["username"] != null) Log("kijelentkezett");
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
            wm.writer_Date = "";
            if (name.Length < 1) name = name2;
            if (name.Length < 1 && name2.Length < 1)
            {
                name = "anonimus";
                name2 = "";
            }
           
            wm.Id = db_book.Writer.Max(q=>q.Id) +1;
            wm.writer_name = name;
            wm.real_name = name2;
            wm.aboutpath = "";
            db_book.Writer.Add(wm);
            db_book.SaveChanges();
            Log("hozzáadott egy Írót", wm.Id+"");
            return View("TheMetaViewer");
        }
        public ActionResult CreateCategory(string name)
        {
            Categories cp = new Categories();
            cp.Id = db_book.Categories.Max(q => q.Id) + 1;
            cp.Name = name;
            db_book.Categories.Add(cp);
            db_book.SaveChanges();
            Log("hozzáadott egy kategoriát", cp.Id+"");
            return View("TheMetaViewer");
        }
        //public ActionResult CreateAuthor(string name ,string life)
        //{
        //    Author ath = new Author();
        //    ath.Id = db_book.Author.Max(q => q.Id) + 1;
        //    ath.name = name;
        //    ath.Date = life;
        //    db_book.Author.Add(ath);
        //    db_book.SaveChanges();
        //    Log("hozzáadott egy kiadót", ath.Id+"");
        //    return View("TheMetaViewer");
        //}
        public ActionResult CreateBook(string name, string isbn, string auth,string img,string demo,string categori, int quantity,string date)
        {
            konyv kv = new konyv();
            DateTime addedtime;
            if(img == null)
            {
                img = "0";
            }
            if (img.Length < 9)
            {
                img = "0";
            }
            if (!DateTime.TryParse(date, out addedtime)) addedtime = DateTime.Now;
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
            else
            {
                kv.imageID = 0;
            }
           
            kv.authorId = 0;
            if (auth.Length > 0)
            {
                kv.authorId = db_book.Writer.First(q => q.writer_name == auth).Id;
            }
            kv.Categories = int.Parse(categori);
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
            KonyvPeldany[] kvp = new KonyvPeldany[quantity];
            int startindex = db_book.KonyvPeldany.Where(q=>q.book_id == kv.ISBN).Count();
            for (int i = 0; i < kvp.Length; i++)
            {
                kvp[i] = new KonyvPeldany();
                kvp[i].AddedTime = addedtime;
                kvp[i].book_id = kv.ISBN;
                kvp[i].PeldanyId = startindex;
                startindex++;
            }
            db_book.KonyvPeldany.AddRange(kvp);
            db_book.konyv.Add(kv);
            db_book.SaveChanges();
            Log("hozzáadott egy könyvet", kv.Id+"");
            return View("TheMetaViewer");
        }
        public ActionResult CreateUser(string Uname, string mail, string Upp, string UppR, bool? Adm, string phone)
        {
            //if (Upp != UppR)
            //    return View("Regist");

            User_sus account1 = new User_sus();
            user account2 = new user();
            //this should be fine until i find out to use auto increment.
            //account.id = bullshit.user.Count() + 1;
            account1.Username = Uname;
            account2.Username = Uname;
            account1.Userpassword = Upp;
            account1.email = mail;
            account1.phone = phone;
            if (Adm == null)
                Adm = false;
            account2.admin = (bool)Adm ? 2 : 1;
            account2.special_password = Upp;
            //try
            //{
            db_user.User_sus.Add(account1);
            db_user.SaveChanges();

            account2.user_id = db_user.User_sus.Where(q => q.Username == account1.Username && q.email == account1.email).FirstOrDefault().Id;
            db_book.user.Add(account2);

            db_book.SaveChanges();

            //Session["username"] = Uname;
            //}
            //catch (Exception)
            //{
            //   // return View("Regist");
            //}
            Log("Létrehozott egy Dologozót", account2.user_id + "");
            //Session.Clear();
            return View("Regist");
        }
        public ActionResult CreateReader(string name,string name2, string mail, string Upp, string UppR, string phone, string szid,string home,string birthday,string birthpalace)
        {
            if (Upp != UppR)
                return View("Regist");

            User_sus account1 = new User_sus();
            user account2 = new user();
            Reader_Card rc = new Reader_Card();
            //this should be fine until i find out to use auto increment.
            //account.id = bullshit.user.Count() + 1;
            account1.Username = name;
            account2.Username = name;
            account1.Userpassword =Upp;
            account1.email = mail;
            account1.phone = phone;
            account2.admin = 0;
            account2.special_password = Upp;
            db_user.User_sus.Add(account1);
            db_user.SaveChanges();

            account2.user_id = db_user.User_sus.Where(q => q.Username == account1.Username && q.email == account1.email).FirstOrDefault().Id;
            rc.Personel_ID_Card = szid;
            rc.home = home;
            DateTime addedtime;
            if (DateTime.TryParse(birthday, out addedtime)) addedtime = DateTime.Now;
            rc.Birthday = addedtime;
            rc.Birtpalace = birthpalace;
            //todo change every id to int!!!
            rc.User_ID = account2.user_id;
            //?why is this even in here?
            ////rc.Rent_ID_Bundle 
            db_book.user.Add(account2);
            db_book.Reader_Card.Add(rc);
            db_book.SaveChanges();

            //Session["username"] = Uname;
            //}
            //catch (Exception)
            //{
            //   // return View("Regist");
            //}
            Log("Létrehozott egy Tagot", account2.user_id + "");
            return View("reader_card");
        }

        public ActionResult CreateRent(string id,string book_id)
        {
            Rent rent = new Rent();
            rent.Card_ID = int.Parse(id);
            rent.Book_ID = book_id;
            rent.Rent_Date = DateTime.Now;
            rent.Due_Date = DateTime.Now.AddDays(14);
            int wichkonyv = int.Parse(book_id);
            db_book.konyv.Where(q => q.Id == wichkonyv).First().Available_Quantity -= 1;
            db_book.Rent.Add(rent);
            db_book.SaveChanges();
            Log("Kiadot egy könyvet", id + "");
            return View("TheMetaViewer");
        }

        public ActionResult DelWriter(string name)
        {
            int data = int.Parse(name);
            Writer wm = db_book.Writer.Where(q => q.Id == data).First();
            db_book.Writer.Remove(wm);
            db_book.SaveChanges();
            Log("Törölt egy Írót", wm.Id + "");
            return View("TheMetaViewer");
        }
        public ActionResult DelCategory(string name)
        {
            Categories cp = db_book.Categories.Where(c => c.Name == name).First();
            db_book.Categories.Remove(cp);
            db_book.SaveChanges();
            Log("Törölt egy kategoriát", cp.Id + "");
            return View("TheMetaViewer");
        }
        //public ActionResult CreateAuthor(string name ,string life)
        //{
        //    Author ath = new Author();
        //    ath.Id = db_book.Author.Max(q => q.Id) + 1;
        //    ath.name = name;
        //    ath.Date = life;
        //    db_book.Author.Add(ath);
        //    db_book.SaveChanges();
        //    Log("hozzáadott egy kiadót", ath.Id+"");
        //    return View("TheMetaViewer");
        //}
        public ActionResult Delbooks(string name)
        {
                string namesplit = name.Split(';')[0];
                int wwwm = int.Parse(name.Split(';')[1]);
               /* KonyvPeldany kmp =*/ db_book.KonyvPeldany.Where(q => q.book_id == namesplit).Where(q => q.PeldanyId == wwwm).First().RemovedTime = DateTime.Now;

                //kmp.RemovedTime= DateTime.Now;
                //db_book.KonyvPeldany.Where(q => q.book_id == namesplit).Where(q => q.PeldanyId == wwwm).First() = kmp;
                //for (int i = 0; i < namesplit.Length; i++)
                //{
                //    if (namesplit[i].Length <= 0) continue;
                //    int tempid = int.Parse(namesplit[i]);
                //    konyv torolj = bullshit.konyv.Where(q => q.Id.Equals(tempid)).FirstOrDefault();
                //    Log("Törölt egy könyvet", torolj.Id + "");
                //    bullshit.konyv.Remove(torolj);

                //}
                db_book.SaveChanges();

            return View("index");
        }
        //public ActionResult DelUser(string Uname, string mail, string Upp, string UppR, bool? Adm, string phone)
        //{
        //    //if (Upp != UppR)
        //    //    return View("Regist");

        //    User_sus account1 = new User_sus();
        //    user account2 = new user();
        //    //this should be fine until i find out to use auto increment.
        //    //account.id = bullshit.user.Count() + 1;
        //    account1.Username = Uname;
        //    account2.Username = Uname;
        //    account1.Userpassword = Upp;
        //    account1.email = mail;
        //    account1.phone = phone;
        //    if (Adm == null)
        //        Adm = false;
        //    account2.admin = (bool)Adm ? 2 : 1;
        //    account2.special_password = Upp;
        //    //try
        //    //{
        //    db_user.User_sus.Add(account1);
        //    db_user.SaveChanges();

        //    account2.user_id = db_user.User_sus.Where(q => q.Username == account1.Username && q.email == account1.email).FirstOrDefault().Id;
        //    db_book.user.Add(account2);

        //    db_book.SaveChanges();

        //    //Session["username"] = Uname;
        //    //}
        //    //catch (Exception)
        //    //{
        //    //   // return View("Regist");
        //    //}
        //    Log("Létrehozott egy Dologozót", account2.user_id + "");
        //    //Session.Clear();
        //    return View("index");
        //}
        //public ActionResult DelReader(string name, string mail, string Upp, string UppR, string phone, string szid)
        //{
        //    if (Upp != UppR)
        //        return View("Regist");

        //    User_sus account1 = new User_sus();
        //    user account2 = new user();
        //    Reader_Card rc = new Reader_Card();
        //    //this should be fine until i find out to use auto increment.
        //    //account.id = bullshit.user.Count() + 1;
        //    account1.Username = name;
        //    account2.Username = name;
        //    account1.Userpassword = Upp;
        //    account1.email = mail;
        //    account1.phone = phone;
        //    account2.admin = 0;
        //    account2.special_password = Upp;
        //    db_user.User_sus.Add(account1);
        //    db_user.SaveChanges();

        //    account2.user_id = db_user.User_sus.Where(q => q.Username == account1.Username && q.email == account1.email).FirstOrDefault().Id;
        //    rc.Personel_ID_Card = szid;
        //    //todo change every id to int!!!
        //    rc.User_ID = account2.user_id;
        //    //?why is this even in here?
        //    ////rc.Rent_ID_Bundle 
        //    db_book.user.Add(account2);
        //    db_book.Reader_Card.Add(rc);
        //    db_book.SaveChanges();

        //    //Session["username"] = Uname;
        //    //}
        //    //catch (Exception)
        //    //{
        //    //   // return View("Regist");
        //    //}
        //    Log("Létrehozott egy Tagot", account2.user_id + "");
        //    return View("TheMetaViewer");
        //}
        public ActionResult delRent(string id)
        {
            Rent rent = db_book.Rent.Where(q=>q.Id == int.Parse(id)).First();
            rent.Return_Date = DateTime.Now;
            int wichkonyv = int.Parse(rent.Book_ID);
            db_book.konyv.Where(q => q.Id == wichkonyv ).First().Available_Quantity += 1;
            db_book.SaveChanges();
            Log("visszahozott egy könyvet", id + "");
            return View("TheMetaViewer");
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

        public string RenderBook(konyv item)
        {
            return $" <tr  onclick=\"tbclick(this)\" ondblclick=\"tbdbclick(this)\"> <td>{item.Id} </td>\r\n                    <td>{item.ISBN}</td>\r\n                    <td> {db_book.Writer.Where(q => q.Id == item.authorId).FirstOrDefault().writer_name} </td>\r\n                    <td>{item.name}</td>\r\n                          <td>{db_book.Categories.Where(q => q.Id == item.Categories).FirstOrDefault().Name}</td>\r\n                    {"<td>" + db_book.KonyvPeldany.Where(q => q.book_id == item.ISBN).Count(q => !q.RemovedTime.HasValue) + "</td>"}    \r\n                </tr> ";
        }
        public string RenderBookItem(string isbn)
        {
            string issbn = isbn.Split(';')[0];
            konyv item = db_book.konyv.First(q=>q.ISBN == issbn);
            return $" <tr  onclick=\"tbclick(this)\" ondblclick=\"tbdbclick(this)\"> <td>{item.Id} </td>\r\n                    <td>{item.ISBN}</td>\r\n                    <td> {db_book.Writer.Where(q => q.Id == item.authorId).FirstOrDefault().writer_name} </td>\r\n                    <td>{item.name}</td>\r\n                          <td>{db_book.Categories.Where(q => q.Id == item.Categories).FirstOrDefault().Name}</td>\r\n                    {"<td>" + db_book.KonyvPeldany.Where(q => q.book_id == item.ISBN).Count(q => !q.RemovedTime.HasValue) + "</td>"}    \r\n                </tr> ";

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
                            string compare = item.Id.ToString();
                            if (item.authorId.Equals(wr.Id))
                                //html_code += $" <tr  onclick=\"tbclick(this)\" ondblclick=\"tbdbclick(this)\"> <td>{item.Id} </td>\r\n                    <td>{item.ISBN}</td>\r\n                    <td> {db_book.Writer.Where(q => q.Id == item.authorId).FirstOrDefault().writer_name} </td>\r\n                    <td>{item.name}</td>\r\n                    <td> <img src=\"/Default/Load_Image_File_Id/{item.imageID}\" alt=\"Alternate Text\" height=\"50px\" /> </td>\r\n                    {"<td>" + item.Quantity + "</td>"}    \r\n                </tr> ";
                                html_code += RenderBook(item);

                        }
                    }
                }
                if (html_code.Length < 3) { html_code = "Ez a szerző nem szerepel nálunk!"; }
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
            string compare = "";

            foreach (var item in db_book.konyv)
            {
                if (item.name.ToLower().Contains(search.ToLower()))
                {
                    occurances++;
                    compare = item.Id.ToString();
                    //html_code += $" <tr  onclick=\"tbclick(this)\" ondblclick=\"tbdbclick(this)\"> <td>{ item.Id } </td>\r\n                    <td>{item.ISBN}</td>\r\n                    <td> {db_book.Writer.Where(q => q.Id == item.authorId).FirstOrDefault().writer_name} </td>\r\n                    <td>{item.name}</td>\r\n                    <td> <img src=\"/Default/Load_Image_File_Id/{item.imageID}\" alt=\"Alternate Text\" height=\"50px\" /> </td>\r\n                    { "<td>" + item.Quantity + "</td>" }    \r\n                </tr> ";
                    html_code += RenderBook(item);
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
            if (html_code.Length < 3) { html_code = "Ez a mű jelenleg nem szerepel az állományunkban";
            }
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
            string compare = "";
            foreach (var item in db_book.konyv)
            {
                if (item.ISBN.Contains(search))
                {
                    compare = item.Id.ToString();
                    //html_code += $" <tr  onclick=\"tbclick(this)\" ondblclick=\"tbdbclick(this)\"> <td>{item.Id} </td>\r\n                    <td>{item.ISBN}</td>\r\n                    <td> {db_book.Writer.Where(q => q.Id == item.authorId).FirstOrDefault().writer_name} </td>\r\n                    <td>{item.name}</td>\r\n                    <td> <img src=\"/Default/Load_Image_File_Id/{item.imageID}\" alt=\"Alternate Text\" height=\"50px\" /> </td>\r\n                    {"<td>" + item.Quantity + "</td>"}    \r\n                </tr> ";
                    html_code += RenderBook(item);
                }
            }
            if (html_code.Length < 3) { html_code = "Ez a mű jelenleg nem szerepel az állományunkban"; }
            return html_code;
        }
        public String LiveSearchCategries(string search)
        {

            search = search.Trim();
            string html_code = "";
            if (search.Length < 1)
            {
                return AllBook(search);
            }
            string compare = "";
            foreach (var item in db_book.konyv)
            {
                if (db_book.Categories.First(q=>q.Id== item.Categories).Name.Contains(search))
                {
                    compare = item.Id.ToString();
                    //html_code += $" <tr  onclick=\"tbclick(this)\" ondblclick=\"tbdbclick(this)\"> <td>{item.Id} </td>\r\n                    <td>{item.ISBN}</td>\r\n                    <td> {db_book.Writer.Where(q => q.Id == item.authorId).FirstOrDefault().writer_name} </td>\r\n                    <td>{item.name}</td>\r\n                    <td> <img src=\"/Default/Load_Image_File_Id/{item.imageID}\" alt=\"Alternate Text\" height=\"50px\" /> </td>\r\n                    {"<td>" + item.Quantity + "</td>"}    \r\n                </tr> ";
                    html_code += RenderBook(item);
                }
            }
            if (html_code.Length < 3) { html_code = "Ez a kategoria jelenleg nem szerepel az állományunkban"; }
            return html_code;
        }
        public String LiveSearchReaderCard(string search)
        {

            search = search.Trim();
            string html_code = "";
            if (search.Length < 1)
            {
                return AllBook(search);
            }
            string compare = "";
            int iduser = -1;
            if(!int.TryParse(search,out iduser)) return "Ilyen olvaso nem létezik";
            foreach (var item in db_book.Rent.Where(q=>q.Card_ID==iduser))
            {
                    //html_code += $" <tr  onclick=\"tbclick(this)\" ondblclick=\"tbdbclick(this)\"> <td>{item.Id} </td>\r\n                    <td>{item.ISBN}</td>\r\n                    <td> {db_book.Writer.Where(q => q.Id == item.authorId).FirstOrDefault().writer_name} </td>\r\n                    <td>{item.name}</td>\r\n                    <td> <img src=\"/Default/Load_Image_File_Id/{item.imageID}\" alt=\"Alternate Text\" height=\"50px\" /> </td>\r\n                    {"<td>" + item.Quantity + "</td>"}    \r\n                </tr> ";
                    html_code += RenderBook(db_book.konyv.First(q=>q.ISBN==item.Book_ID));
            }
            if (html_code.Length < 3) { html_code = "Ez a olvasó jelenleg nem kölcsönzött ki könyvet"; }
            return html_code;
        }
        private string AllBook(string search)
        {
            string html_code = "";
            string compare = "";
            //todo create pagenation 25
                 foreach (var item in db_book.konyv)
                {
                compare = item.ISBN.ToString();
                //html_code += $" <tr  onclick=\"tbclick(this)\" ondblclick=\"tbdbclick(this)\"> <td>{item.Id} </td>\r\n                    <td>{item.ISBN}</td>\r\n                    <td> {db_book.Writer.Where(q => q.Id == item.authorId).FirstOrDefault().writer_name} </td>\r\n                    <td>{item.name}</td>\r\n                    <td> <img src=\"/Default/Load_Image_File_Id/{item.imageID}\" alt=\"Alternate Text\" height=\"50px\" /> </td>\r\n                    {"<td>" + item.Quantity + "</td>"}    \r\n                </tr> ";
                html_code += RenderBook(item);
            }
                 if(html_code.Length < 3) { html_code = "A keresett könyv nincs meg nálunk"; }
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
        public ActionResult LogInUser(string Uname,string Upp,string RegYet)
        {
            //if (RegYet != null)
            //    return RegisterUser();
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
                            Session["level"] = db_book.user.Where(q => q.user_id == item.Id).FirstOrDefault().admin;
                            Log(item.Id, "Bejelentkezett");
                            return View("Index");                            
                            }
                        }
                    }
                    catch (Exception e)
                    {
                    }
                   
                }
            Session.Clear();
            return View("error");
        }
        public ActionResult RegisterUserPage()
        {
            return View("Regist");
        }
        public ActionResult Secnd()
        {
            if(Session["username"] != null) Log("kijelentkezett");
            Session.Clear();
            return View("LogIn");
        }
        public void Log(string what)
        {
            Log data = new Log();
            data.who = int.Parse(Session["userid"].ToString());
            data.what = what;
            data.when = DateTime.Now;
            data.whom = "--";
            db_book.Log.Add(data);
            db_book.SaveChanges();
        }
        public void Log(int id, string what)
        {
            Log data = new Log();
            data.who = id;
            data.what = what;
            data.when = DateTime.Now;
            data.whom = "--";
            db_book.Log.Add(data);
            db_book.SaveChanges();
        }
        public void Log(int id,string what,string whom)
        {
            Log data = new Log();
            data.who = id;
            data.what = what;
            data.when = DateTime.Now;
            data.whom = whom;
            db_book.Log.Add(data);
            db_book.SaveChanges();
        }
        public void Log(string what, string whom)
        {
            Log data = new Log();
            data.who = int.Parse(Session["userid"].ToString());
            data.what = what;
            data.when = DateTime.Now;
            data.whom = whom;
            db_book.Log.Add(data);
            db_book.SaveChanges();
        }
        public string[] KonyvSTR(konyv inp)
        {
            String[] vm = new string[4] { inp.Id.ToString(), inp.ISBN, inp.name, "author" + db_book.Author.Where(q=>q.Id == inp.authorId).First().name   };
            return vm;
        }
        public ActionResult LogView()
        {
            return View("LogView");
        }
    }
}
