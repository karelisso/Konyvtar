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
using System.Security.Cryptography;
using System.Text;

namespace Könyvtár.App_Data
{
    public class DefaultController : Controller
    {
        //public static UsersEntities db_user = new UsersEntities(); db_user.User_sus
        public book_vs19Entities1 db_book = new book_vs19Entities1();
        public string tiltottsessions = "username usermail userid level";
        // GET: Default
        public ActionResult IndexPage()
        {
            if (Session["username"] != null)
            {
                return View("index");
            }

            return startPage();
           
        }
        public ActionResult TagPage()
        {
            if (Session["username"] != null)
            {
                return View("TagAdd");
            }
            return startPage();
        }
        public ActionResult MainPage()
        {
            if (Session["username"] != null)
            {
                return View("main_page");
            }
            return startPage();
        }
        public ActionResult HelperPage()
        {
            if (Session["username"] != null)
                return View("Help");
            return startPage();
        } 
        public ActionResult CreateReaderCardPage()
        {
            if (Session["username"] != null)
                return View("reader_card_create");
            return startPage();
        }
        public ActionResult RentReaderCardPage()
        {
            if (Session["username"] != null)
                return View("reader_card_rent");
            return startPage();
        }
        public ActionResult startPage()
        {
            if (Session["username"] != null) {
                Log("1");
                Session.Clear();
            } 
            
            return View("emptypage");
        }
        public ActionResult CreateMetaPage()
        {
              if (Session["username"] != null)
                return View("TheMetaViewerAdd");
            return startPage();
        }
        public ActionResult DeleteMetaPage()
        {
            if (Session["username"] != null)
                return View("TheMetaViewerRemove");
            return startPage();
        }
        public ActionResult BookPage()
        {
            if (Session["username"] != null)
                return View("book_detail");
            return startPage();
        }
        public ActionResult LogPage()
        {
            if (Session["username"] != null)
                return View("LogView");
            return startPage();
        }
        public ActionResult LogInPage()
        {
                return View("LogIn");
        }
        public ActionResult RegisterUserPage()
        {
            if (Session["username"] != null)
                return View("Regist");
            return startPage();
        }

        public void SetSession(string name, string value)
        {
            if(!tiltottsessions.Contains(name.ToLower()))
            {
                Session[name.Trim()] = null;

                Session[name.Trim()] = value.Trim();
            }
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
           
            wm.IdWriter = db_book.Writer.Max(q=>q.IdWriter) +1;
            wm.writer_name = name;
            wm.real_name = name2;
            wm.aboutpath = "";
            db_book.Writer.Add(wm);
            db_book.SaveChanges();
            Log("2", wm.IdWriter+"");
            return CreateMetaPage();
        }
        public ActionResult CreateCategory(string name)
        {
            Categories cp = new Categories();
            cp.IdCategorie = db_book.Categories.Max(q => q.IdCategorie) + 1;
            cp.Name = name;
            db_book.Categories.Add(cp);
            db_book.SaveChanges();
            Log("3", cp.IdCategorie+"");
            return CreateMetaPage();
        }
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
            if(categori.Length > 0)
            {
                if( db_book.Categories.Count(q => q.Name.Equals(categori))  <=0) CreateCategory(categori);
            }
            if (auth.Length > 0)
            {
                if (db_book.Writer.Count(q => q.writer_name.Equals(auth)) <= 0) CreateWriter(auth,"","","");
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
                kv.authorId = db_book.Writer.First(q => q.writer_name == auth).IdWriter;
            }
            kv.Categories = db_book.Categories.First(q=>q.Name.Equals(categori)).IdCategorie;
            kv.ISBN = isbn;
            kv.name = name;
            kv.demo = demo;
            try
            {
                kv.IdKonyv = db_book.konyv.Max(q => q.IdKonyv) + 1;
            }
            catch (Exception)
            {

                kv.IdKonyv = 0;
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
            Log("5", kv.IdKonyv+"");
            return CreateMetaPage();
        }
        public ActionResult CreateUser(string Uname, string mail, string Upp, string UppR, bool? Adm, string phone)
        {
            if (Upp != UppR)
                return startPage();

            User_sus account1 = new User_sus();
            user account2 = new user();
            //this should be fine until i find out to use auto increment.
            //account.id = bullshit.user.Count() + 1;
            account1.Username = Uname;
            account2.Username = Uname;
            account1.Userpassword = ComputeStringToSha256Hash(Upp);
            account1.email = mail;
            account1.phone = phone;
            if (Adm == null)
                Adm = false;
            account2.admin = (bool)Adm ? 2 : 1;
            account2.special_password = ComputeStringToSha256Hash(Upp);
            //try
            //{
            db_book.User_sus.Add(account1);
            //db_user.SaveChanges();

            account2.user_id = db_book.User_sus.Where(q => q.Username == account1.Username && q.email == account1.email).FirstOrDefault().Id;
            db_book.user.Add(account2);

            db_book.SaveChanges();

            //Session["username"] = Uname;
            //}
            //catch (Exception)
            //{
            //   // return View("Regist");
            //}
            Log("7", account2.user_id + "");
            //Session.Clear();
            return RegisterUserPage();
        }
        public ActionResult CreateReader(string name,string name2, string mail, string Upp, string UppR, string phone, string szid,string home,string birthday,string birthpalace)
        {
            if (Upp != UppR)
                return CreateReaderCardPage();

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
            db_book.User_sus.Add(account1);
            db_book.SaveChanges();

            account2.user_id = db_book.User_sus.Where(q => q.Username == account1.Username && q.email == account1.email).FirstOrDefault().Id;
            rc.Personel_ID_Card = szid;
            rc.home = home;
            DateTime addedtime;
            if (DateTime.TryParse(birthday, out addedtime)) addedtime = DateTime.Now;
            rc.Birthday = addedtime;
            rc.Birtpalace = birthpalace;
            //todo change every id to int!!!
            rc.User_ID = account2.user_id;
            db_book.user.Add(account2);
            db_book.Reader_Card.Add(rc);
            db_book.SaveChanges();
              
            Log("8", account2.user_id + "");
            return CreateReaderCardPage();
        }

        public ActionResult CreateRent(int? szid,string book_id,string date)
        {
            if(Session["currentreadercard"] ==null) return RentReaderCardPage();
            string curuser = Session["currentreadercard"].ToString();

            if (db_book.Reader_Card.Count(q => q.Personel_ID_Card.Equals(curuser)) < 0)
            {
                return RentReaderCardPage();
            }

            //int? current = db_book.Reader_Card.FirstOrDefault(q => q.Personel_ID_Card.Equals(curuser)).User_ID;
            Debug.Write(Session["userid"]);
            Rent rent = new Rent();
            rent.Card_ID = db_book.Reader_Card.First(q => q.Personel_ID_Card.Equals(curuser)).IdReaderCard;// int.Parse( db_book.user.First(q=>q.user_id.Equals(szid)). );
            //string usernaemsplit = username.Split('/')[0];
            //rent.Card_ID = db_book.Reader_Card.First(q => q.Personel_ID_Card.Equals(szid)).Id;//db_book.user.First(q => q.Username.Equals(usernaemsplit)).user_id.Value;
            rent.Book_ID = book_id;

            DateTime addedtime;
            if (!DateTime.TryParse(date, out addedtime)) addedtime = DateTime.Now;
            rent.Rent_Date = addedtime.Date;
            rent.Due_Date = addedtime.AddDays(14).Date;
            //int wichkonyv = int.Parse(book_id);
            //db_book.konyv.Where(q => q.Id == wichkonyv).First().Available_Quantity -= 1;
            db_book.Rent.Add(rent);
            db_book.SaveChanges();
            Log("9", book_id + "");
            return RentReaderCardPage();
        }

        public ActionResult DelWriter(string name)
        {
            int data = int.Parse(name);
            Writer wm = db_book.Writer.Where(q => q.IdWriter == data).First();
            db_book.Writer.Remove(wm);
            db_book.SaveChanges();
            Log("Törölt egy Írót", wm.IdWriter + "");
            return DeleteMetaPage();
        }
        public ActionResult DelCategory(string name)
        {
            Categories cp = db_book.Categories.Where(c => c.Name == name).First();
            db_book.Categories.Remove(cp);
            db_book.SaveChanges();
            Log("Törölt egy kategoriát", cp.IdCategorie + "");
            return DeleteMetaPage();
        }

        public ActionResult Delbooks(string name)
        {
                string namesplit = name.Split(';')[0];
                int wwwm = int.Parse(name.Split(';')[1]);
               db_book.KonyvPeldany.Where(q => q.book_id == namesplit).Where(q => q.PeldanyId == wwwm).First().RemovedTime = DateTime.Now;
                db_book.SaveChanges();
            Log("6",name);
            return IndexPage();
        }
      
        public ActionResult delRent(string bookid,string date)
        {
            string[] bookidsplit = bookid.Split(';');
            Rent rent = db_book.Rent.First(q => q.Book_ID.Equals(bookid));
            rent.Return_Date = DateTime.Now;
            DateTime addedDate;
            if (!DateTime.TryParse(date, out addedDate)) addedDate = DateTime.Now;
            rent.Return_Date = addedDate;
            string wichkonyv = rent.Book_ID.Split(';')[0];
            db_book.konyv.Where(q => q.ISBN == wichkonyv ).First().Available_Quantity += 1;
            db_book.SaveChanges();
            Log("10", bookid + "");
            return RentReaderCardPage();
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
            return $" <tr  onclick=\"tbclick(this)\" ondblclick=\"tbdbclick(this)\"> <td>{item.IdKonyv} </td>\r\n                    <td>{item.ISBN}</td>\r\n                    <td> {db_book.Writer.Where(q => q.IdWriter == item.authorId).FirstOrDefault().writer_name} </td>\r\n                    <td>{item.name}</td>\r\n                          <td>{db_book.Categories.Where(q => q.IdCategorie == item.Categories).FirstOrDefault().Name}</td>\r\n                    {"<td>" + db_book.KonyvPeldany.Where(q => q.book_id == item.ISBN).Count(q => !q.RemovedTime.HasValue) + "</td>"}    \r\n                </tr> ";
        }
        public string RenderBookItem(string isbn)
        {
            string issbn = isbn.Split(';')[0];
            konyv item = db_book.konyv.First(q=>q.ISBN == issbn);
            return $" <tr  onclick=\"tbclick(this)\" ondblclick=\"tbdbclick(this)\"> <td>{item.IdKonyv} </td>\r\n                    <td>{item.ISBN}</td>\r\n                    <td> {db_book.Writer.Where(q => q.IdWriter == item.authorId).FirstOrDefault().writer_name} </td>\r\n                    <td>{item.name}</td>\r\n                          <td>{db_book.Categories.Where(q => q.IdCategorie == item.Categories).FirstOrDefault().Name}</td>\r\n                    {"<td>" + db_book.KonyvPeldany.Where(q => q.book_id == item.ISBN).Count(q => !q.RemovedTime.HasValue) + "</td>"}    \r\n                </tr> ";

        }
        public string LiveSearchWriter(string search = "")
        {

            search = search.Trim();
            if (search.Length < 1)
            {
                return AllBook();
            }
            string html_code = "";
            int occurances = 0;
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
                            string compare = item.IdKonyv.ToString();
                            if (item.authorId.Equals(wr.IdWriter))
                                //html_code += $" <tr  onclick=\"tbclick(this)\" ondblclick=\"tbdbclick(this)\"> <td>{item.Id} </td>\r\n                    <td>{item.ISBN}</td>\r\n                    <td> {db_book.Writer.Where(q => q.Id == item.authorId).FirstOrDefault().writer_name} </td>\r\n                    <td>{item.name}</td>\r\n                    <td> <img src=\"/Default/Load_Image_File_Id/{item.imageID}\" alt=\"Alternate Text\" height=\"50px\" /> </td>\r\n                    {"<td>" + item.Quantity + "</td>"}    \r\n                </tr> ";
                                html_code += RenderBook(item);

                        }
                    }
                }
                if (html_code.Length < 3) { html_code = "Ez a szerző nem szerepel nálunk!"; }
                return html_code;
        }

        public string LiveSearchName(string search = "")
        {

            search = search.Trim();
            if (search.Length < 1)
            {
                return AllBook();
            }
            string html_code = "";
            int occurances = 0;
            string compare = "";

            foreach (var item in db_book.konyv)
            {
                if (item.name.ToLower().Contains(search.ToLower()))
                {
                    occurances++;
                    compare = item.IdKonyv.ToString();
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

        public string LiveSearchISBN(string search = "")
        {

            search = search.Trim();
            string html_code = "";
            if (search.Length < 1)
            {
                return AllBook();
            }
            string compare = "";
            foreach (var item in db_book.konyv)
            {
                if (item.ISBN.Contains(search))
                {
                    compare = item.IdKonyv.ToString();
                    //html_code += $" <tr  onclick=\"tbclick(this)\" ondblclick=\"tbdbclick(this)\"> <td>{item.Id} </td>\r\n                    <td>{item.ISBN}</td>\r\n                    <td> {db_book.Writer.Where(q => q.Id == item.authorId).FirstOrDefault().writer_name} </td>\r\n                    <td>{item.name}</td>\r\n                    <td> <img src=\"/Default/Load_Image_File_Id/{item.imageID}\" alt=\"Alternate Text\" height=\"50px\" /> </td>\r\n                    {"<td>" + item.Quantity + "</td>"}    \r\n                </tr> ";
                    html_code += RenderBook(item);
                }
            }
            if (html_code.Length < 3) { html_code = "Ez a mű jelenleg nem szerepel az állományunkban"; }
            return html_code;
        }
        public string LiveSearchCategries(string search = "")
        {

            search = search.Trim();
            string html_code = "";
            if (search.Length < 1)
            {
                return AllBook();
            }
            string compare = "";
            foreach (var item in db_book.konyv)
            {
                if (db_book.Categories.First(q=>q.IdCategorie== item.Categories).Name.Contains(search))
                {
                    compare = item.IdKonyv.ToString();
                    //html_code += $" <tr  onclick=\"tbclick(this)\" ondblclick=\"tbdbclick(this)\"> <td>{item.Id} </td>\r\n                    <td>{item.ISBN}</td>\r\n                    <td> {db_book.Writer.Where(q => q.Id == item.authorId).FirstOrDefault().writer_name} </td>\r\n                    <td>{item.name}</td>\r\n                    <td> <img src=\"/Default/Load_Image_File_Id/{item.imageID}\" alt=\"Alternate Text\" height=\"50px\" /> </td>\r\n                    {"<td>" + item.Quantity + "</td>"}    \r\n                </tr> ";
                    html_code += RenderBook(item);
                }
            }
            if (html_code.Length < 3) { html_code = "Ez a kategoria jelenleg nem szerepel az állományunkban"; }
            return html_code;
        }
        public string LiveSearchReaderCard(string search = "")
        {

            search = search.Trim();
            string html_code = "";
            if (search.Length < 1)
            {
                return AllBook();
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
        //public ActionResult RenderReaderDetails(string szid)
        //{
        //    Reader_Card rc = db_book.Reader_Card.FirstOrDefault(q => q.Personel_ID_Card == szid);
        //    if (rc == null) Session["readerCard"] = "";
        //    else Session["readerCard"] = $"{rc.Personel_ID_Card}";
        //    return ReaderCard();
        //}
        public string AllBook()
        {
            string html_code = "";
            //todo create pagenation 25
                 foreach (var item in db_book.konyv)
                {
                html_code += RenderBook(item);
            }
                 if(html_code.Length < 3) { html_code = "A keresett könyv nincs meg nálunk"; }
            return html_code;
        }
        public string GetReaderCard()
        {
            string curuser = Session["currentreadercard"].ToString();

            if(db_book.Reader_Card.Count(q => q.Personel_ID_Card.Equals(curuser)) > 0)
            {
                int? current = db_book.Reader_Card.FirstOrDefault(q => q.Personel_ID_Card.Equals(curuser)).User_ID;
                return $" <tr> <td> <label>Név</label>  </td> <td>  {db_book.user.First(q => q.user_id == current).Username}</td>\r\n                        </tr>\r\n\r\n                        <tr>\r\n                            <td>  <label>lakcím</label>  </td>\r\n                            <td>  {db_book.Reader_Card.First(q => q.User_ID == current).Birtpalace}  </td>\r\n                        </tr>\r\n                        <tr>\r\n                            <td> <label>tel</label>   </td>\r\n                            <td>  {db_book.User_sus.First(q => q.Id == current).phone}  </td>\r\n                        </tr>\r\n                        <tr>\r\n                            <td>  <label>Email cím</label>   </td>\r\n                            <td>  {db_book.User_sus.First(q => q.Id == current).email} </td>\r\n                        </tr>";
            }
            else
            {
               return $"<tr> <td>Nincs ilyen olvasó fölvéve</td> </tr>";
            }
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
            foreach (var item in db_book.User_sus)
                {
                try
                {
                    if (item.Username.ToLower() == Uname.ToLower() || item.email.ToLower() == Uname.ToLower())
                        {
                    
                        if (item.Userpassword == ComputeStringToSha256Hash(Upp))
                            {
                            Session["username"] = Uname;
                            Session["usermail"] = item.email;
                            Session["userid"] = item.Id;
                            Session["level"] =  db_book.user.First(q=>q.user_id==item.Id).admin;
                            Log(item.Id, "0");
                            return IndexPage();                            
                            }
                        }
            }
                    catch (Exception e)
            {
            }

        }

           
            return startPage();
        }


        public void Log(string what)
        {
            Log data = new Log();
            data.who = int.Parse(Session["userid"].ToString());
            data.what = what;
            data.when = DateTime.Now;
            data.whom = "--";
            db_book.Log.Add(data);
            db_book.SaveChangesAsync();
        }
        public void Log(int id, string what)
        {
            Log data = new Log();
            data.who = id;
            data.what = what;
            data.when = DateTime.Now;
            data.whom = "--";
            db_book.Log.Add(data);
            db_book.SaveChangesAsync();
        }
        public void Log(int id,string what,string whom)
        {
            Log data = new Log();
            data.who = id;
            data.what = what;
            data.when = DateTime.Now;
            data.whom = whom;
            db_book.Log.Add(data);
            db_book.SaveChangesAsync();
        }
        public void Log(string what, string whom)
        {
            Log data = new Log();
            data.who = int.Parse(Session["userid"].ToString());
            data.what = what;
            data.when = DateTime.Now;
            data.whom = whom;
            db_book.Log.Add(data);
            db_book.SaveChangesAsync();
        }
        public string[] KonyvSTR(konyv inp)
        {
            String[] vm = new string[4] { inp.IdKonyv.ToString(), inp.ISBN, inp.name, "author" + db_book.Author.Where(q=>q.IdAuthor == inp.authorId).First().name   };
            return vm;
        }


        static string ComputeStringToSha256Hash(string plainText)
        {
            string salt = "13579";
            plainText += salt;
            // Create a SHA256 hash from string   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Computing Hash - returns here byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(plainText));

                // now convert byte array to a string   
                StringBuilder stringbuilder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    stringbuilder.Append(bytes[i].ToString("x2"));
                }
                return stringbuilder.ToString();
            }
        }

    }
}
