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
using System.Threading;
using System.Web.UI.WebControls;
using System.Data.Entity;

namespace Könyvtár.App_Data
{
    public class DefaultController : Controller
    {
        //public static UsersEntities db_user = new UsersEntities(); db_user.User_sus
        public book_vs19Entities1 db_book = new book_vs19Entities1();
        public string tiltottsessions = "username usermail userid level "; // error success
        // GET: Default
        public async Task<ActionResult> IndexPage()
        {
            if (Session["username"] != null)
            {
                return View("index");
            }

            return await startPage();

        }
        public async Task<ActionResult> TagPage()
        {
            if (Session["username"] != null)
            {
                return View("TagAdd");
            }
            return await startPage();
        }
        public async Task<ActionResult> MainPage()
        {
            if (Session["username"] != null)
            {
                return View("main_page");
            }
            return await startPage();
        }
        public async Task<ActionResult> HelperPage()
        {
            if (Session["username"] != null)
                return View("Help");
            return await startPage();
        }
        public async Task<ActionResult> CreateReaderCardPage()
        {
            if (Session["username"] != null)
                return View("reader_card_create");
            return await startPage();
        }
        public async Task<ActionResult> ManageReaderCardPage()
        {
            if (Session["username"] != null)
                return View("reader_card_all");
            return await startPage();
        }

        public async Task<ActionResult> RentReaderCardPage()
        {
            if (Session["username"] != null)
                return View("reader_card_rent");
            return await startPage();
        }
        public async Task<ActionResult> startPage()
        {
            if (Session["username"] != null) {
                await Log("1");
                Session.Clear();
            }

            return View("emptypage");
        }
        public async Task<ActionResult> CreateMetaPage()
        {
            if (Session["username"] != null)
                return View("TheMetaViewerAdd");
            return await startPage();
        }
        public async Task<ActionResult> ChangeMetaPage()
        {
            if (Session["username"] != null)
                return View("TheMetaViewerChange");
            return await startPage();
        }

        public async Task<ActionResult> DeleteMetaPage()
        {
            if (Session["username"] != null)
                return View("TheMetaViewerRemove");
            return await startPage();
        }
        public async Task<ActionResult> BookPage()
        {
            if (Session["username"] != null)
                return View("book_detail");
            return await startPage();
        }
        public async Task<ActionResult> LogPage()
        {
            if (Session["username"] != null)
                return View("LogView");
            return await startPage();
        }
        public async Task<ActionResult> LogInPage()
        {
            return View("LogIn");
        }
        public async Task<ActionResult> RegisterUserPage()
        {
            if (Session["username"] != null)
                return View("Regist");
            return await startPage();
        }

        public async Task<ActionResult> ProfilePage()
        {
            if (Session["username"] != null)
                return View("Profile");
            return await startPage();
        }

        public void SetSession(string name, string value)
        {
            if (!tiltottsessions.Contains(name.ToLower()))
            {
                Session[name.Trim()] = null;

                Session[name.Trim()] = value.Trim();
            }
        }
        public string GetSession(string name)
        {

            return Session[name]?.ToString();
        }

        public async Task<ActionResult> CreateWriter(string name, string name2, string life, string about)
        {
            Writer wm = new Writer();
            wm.writer_Date = "";
            if (name.Length < 1) name = name2;
            if (name.Length < 1 && name2.Length < 1)
            {
                name = "anonimus";
                name2 = "";
            }

            wm.IdWriter = db_book.Writer.Max(q => q.IdWriter) + 1;
            wm.writer_name = name;
            wm.real_name = name2;
            wm.aboutpath = "";
            db_book.Writer.Add(wm);
            await SaveDatabaseBook();
            await Log("2", wm.IdWriter + "");
            return await CreateMetaPage();
        }
        public async Task<ActionResult> CreateCategory(string name)
        {
            Categories cp = new Categories();
            cp.IdCategorie = db_book.Categories.Max(q => q.IdCategorie) + 1;
            cp.Name = name;
            db_book.Categories.Add(cp);
            await SaveDatabaseBook();
            await Log("3", cp.IdCategorie + "");
            return await CreateMetaPage();
        }
        public async Task<ActionResult> CreateBook(string name, string isbn, string auth, string img, string demo, string categori, int quantity, string date)
        {
            konyv kv = db_book.konyv.FirstOrDefault(q => q.ISBN.Equals(isbn));
            bool isNewEntry = kv == null;
            if (isNewEntry) kv = new konyv();
            DateTime addedtime;
            if (categori.Length > 0)
            {
                if (db_book.Categories.Count(q => q.Name.Equals(categori)) <= 0) await CreateCategory(categori);
            }
            if (auth.Length > 0)
            {
                if (db_book.Writer.Count(q => q.writer_name.Equals(auth)) <= 0) await CreateWriter(auth, "", "", "");
            }
            if (!DateTime.TryParse(date, out addedtime)) addedtime = DateTime.Now;
            //long? pictureimage;
            if (Request.Files.Count > 0)
            {
                using (var binaryReader = new BinaryReader(Request.Files[0].InputStream))
                {
                    var imageData = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
                    Images imageadd = new Images();
                    imageadd.JPG = imageData;
                    db_book.Images.Add(imageadd);
                    //imageData = imageadd.JPG;
                    int nothing = await SaveDatabaseBook();

                    kv.imageID = db_book.Images.Where(q => q.JPG == imageData).First().Id;


                }
            }
            else if (isNewEntry)
            {
                kv.imageID = 0;
            }
            if (isNewEntry) kv.authorId = 0;
            if (auth.Length > 0)
            {
                kv.authorId = db_book.Writer.First(q => q.writer_name == auth).IdWriter;
            }
            if (categori.Length > 0) kv.Categories = db_book.Categories.First(q => q.Name.Equals(categori)).IdCategorie;
            if (isbn.Length > 9) kv.ISBN = isbn;
            if (name.Length > 0) kv.name = name;
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
            int startindex = db_book.KonyvPeldany.Where(q => q.book_id == kv.ISBN).Count();
            for (int i = 0; i < kvp.Length; i++)
            {
                kvp[i] = new KonyvPeldany();
                kvp[i].AddedTime = addedtime;
                kvp[i].book_id = kv.ISBN;
                kvp[i].PeldanyId = startindex;
                startindex++;
            }
            db_book.KonyvPeldany.AddRange(kvp);
            if (isNewEntry) db_book.konyv.Add(kv);
            await SaveDatabaseBook();
            await Log("5", kv.IdKonyv + "");
            return await CreateMetaPage();
        }
        public async Task<ActionResult> CreateUser(string Uname, string mail, string Upp, string UppR, bool? Adm, string phone)
        {
            if (Upp != UppR)
                return await startPage();

            User_sus account1 = db_book.User_sus.Where(q => q.Username == Uname).FirstOrDefault();
            user account2 = db_book.user.Where(q => q.Username == Uname).FirstOrDefault();
            bool isNewEntry = account1 == null;
            if (isNewEntry)
            {
                if (Upp.Length < 3)
                    return await startPage();
                account1 = new User_sus();
                account2 = new user();
            }
            account1.Username = Uname;
            account2.Username = Uname;
            if (Upp.Length > 3) account1.Userpassword = ComputeStringToSha256Hash(Upp);

            if (mail.Length > 3) account1.email = mail;

            if (phone.Length > 3) account1.phone = phone;
            if (Adm == null)
                Adm = false;
            if (isNewEntry || Adm == true) account2.admin = (bool)Adm ? 2 : 1;

            account2.special_password = ComputeStringToSha256Hash(Upp);
            if (isNewEntry) db_book.User_sus.Add(account1);
            await SaveDatabaseBook();

            if (isNewEntry)
            {
                account2.user_id = db_book.User_sus.Where(q => q.Username == account1.Username && q.email == account1.email).FirstOrDefault().Id;
                db_book.user.Add(account2);
            }
            await SaveDatabaseBook();
            await Log("7", account2.user_id + "");
            return await RegisterUserPage();
        }
        public async Task<ActionResult> CreateReader(string Uname, string name2, string mail, string Upp, string UppR, string phone, string szid, string home, string birthday, string birthpalace)
        {
            if (Upp != UppR)
            {
                Console.WriteLine("problem");
                return await CreateReaderCardPage();
            }


            User_sus account1 = db_book.User_sus.Where(q => q.Username == Uname).FirstOrDefault();
            user account2 = db_book.user.Where(q => q.Username == Uname).FirstOrDefault();
            Reader_Card rc = new Reader_Card();
            bool isNewEntry = account1 == null;
            if (isNewEntry)
            {
                if (Upp.Length < 3)
                    return await startPage();
                account1 = new User_sus();
                account2 = new user();

            }
            else
            {
                rc = db_book.Reader_Card.Where(q => q.User_ID == account1.Id).FirstOrDefault();
            }
            account1.Username = Uname;
            account2.Username = Uname;
            if (Upp.Length > 3) account1.Userpassword = ComputeStringToSha256Hash(Upp);
            if (mail.Length > 3) account1.email = mail;
            if (phone.Length > 3) account1.phone = phone;
            account2.admin = 0;
            account2.special_password = ComputeStringToSha256Hash(Upp);
            if (isNewEntry) db_book.User_sus.Add(account1);
            await SaveDatabaseBook();

            account2.user_id = db_book.User_sus.Where(q => q.Username == account1.Username && q.email == account1.email).FirstOrDefault().Id;
            if (isNewEntry) rc.Personel_ID_Card = szid;
            if (home.Length > 3) rc.home = home;
            DateTime addedtime;
            if (DateTime.TryParse(birthday, out addedtime)) addedtime = DateTime.Now;
            if (isNewEntry) rc.Birthday = addedtime;
            if (isNewEntry || birthpalace.Length > 1) rc.Birtpalace = birthpalace;
            if (name2.Length > 3) rc.Momname = name2;
            if (isNewEntry) rc.User_ID = account2.user_id;
            if (isNewEntry) db_book.user.Add(account2);
            if (isNewEntry) db_book.Reader_Card.Add(rc);
            await SaveDatabaseBook();

            await Log("8", account2.user_id + "");
            return await CreateReaderCardPage();
        }

        public async Task<ActionResult> CreateRent(int? szid, string book_id, string date)
        {
            if (Session["currentreadercard"] == null) {
                Session["error"] = db_book.MessagesError.First(q => q.Id == 5).message;
                return await RentReaderCardPage();
            }
            string curuser = Session["currentreadercard"].ToString();

            if (db_book.Reader_Card.Count(q => q.Personel_ID_Card.Equals(curuser)) < 0)
            {
                Session["error"] = db_book.MessagesError.First(q => q.Id == 5).message;

                return await RentReaderCardPage();
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
            string isbn = book_id.Split(';')[0];
            long db = long.Parse(book_id.Split(';')[1]);
            db_book.KonyvPeldany.Where(q => q.book_id == isbn).First(q => q.PeldanyId == db).isBorrowed = true;


            db_book.Rent.Add(rent);
            await SaveDatabaseBook();
            await Log("9", book_id + "");
            Session["success"] = db_book.MessagesSucces.First(q => q.Id == 2).Message + $" {rent.Book_ID}";
            return await RentReaderCardPage();
        }

        public async Task<ActionResult> DelWriter(string name)
        {
            int data = int.Parse(name);
            Writer wm = db_book.Writer.Where(q => q.IdWriter == data).First();
            db_book.Writer.Remove(wm);
            await SaveDatabaseBook();
            await Log("Törölt egy Írót", wm.IdWriter + "");
            return await DeleteMetaPage();
        }
        public async Task<ActionResult> DelCategory(string name)
        {
            Categories cp = db_book.Categories.Where(c => c.Name == name).First();
            db_book.Categories.Remove(cp);
            await SaveDatabaseBook();
            await Log("Törölt egy kategoriát", cp.IdCategorie + "");
            return await DeleteMetaPage();
        }

        public async Task<ActionResult> Delbooks(string name)
        {
            string namesplit = name.Split(';')[0];
            int wwwm = int.Parse(name.Split(';')[1]);
            db_book.KonyvPeldany.Where(q => q.book_id == namesplit).Where(q => q.PeldanyId == wwwm).First().RemovedTime = DateTime.Now;
            await SaveDatabaseBook();
            await Log("6", name);
            return await IndexPage();
        }

        public async Task<ActionResult> delRent(string bookid, string date)
        {
            string[] bookidsplit = bookid.Split(';');
            Rent rent = db_book.Rent.First(q => q.Book_ID.Equals(bookid));
            rent.Return_Date = DateTime.Now;
            DateTime addedDate;
            if (!DateTime.TryParse(date, out addedDate)) addedDate = DateTime.Now;
            rent.Return_Date = addedDate;
            db_book.KonyvPeldany.Where(q => q.book_id == bookid).First().isBorrowed = false;
            await SaveDatabaseBook();
            await Log("10", bookid + "");
            return await RentReaderCardPage();
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
            byte[] cover = db_book.Images.Where(q => q.Id == id).FirstOrDefault().JPG;
            if (cover != null)
            {
                return File(cover, "image/jpg");
            }
            else
            {
                return null;
            }
        }
        public System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
        {
            try
            {
                MemoryStream ms = new MemoryStream(byteArrayIn, 0, byteArrayIn.Length);
                ms.Write(byteArrayIn, 0, byteArrayIn.Length);
                return System.Drawing.Image.FromStream(ms, true);//Exception occurs here
            }
            catch { }
            return null;
        }

        public string RenderBook(konyv item)
        {
            return $" <tr  onclick=\"tbclick(this)\" ondblclick=\"tbdbclick(this)\"> <td>{item.IdKonyv} </td>\r\n                    <td>{item.ISBN}</td>\r\n                    <td> {db_book.Writer.Where(q => q.IdWriter == item.authorId).FirstOrDefault().writer_name} </td>\r\n                    <td>{item.name}</td>\r\n                          <td>{db_book.Categories.Where(q => q.IdCategorie == item.Categories).FirstOrDefault().Name}</td>\r\n                    {"<td>" + db_book.KonyvPeldany.Count(q => q.book_id == item.ISBN) + " / " + db_book.KonyvPeldany.Where(q => q.book_id == item.ISBN).Count(q => !q.isBorrowed) + "</td>"}    \r\n                </tr> ";
        }
        public string RenderBookItem(string isbn)
        {
            string issbn = isbn.Split(';')[0];
            konyv item = db_book.konyv.First(q => q.ISBN == issbn);
            return $" <tr  onclick=\"tbclick(this)\" ondblclick=\"tbdbclick(this)\"> <td>{item.IdKonyv} </td>\r\n                    <td>{item.ISBN}</td>\r\n                    <td> {db_book.Writer.Where(q => q.IdWriter == item.authorId).FirstOrDefault().writer_name} </td>\r\n                    <td>{item.name}</td>\r\n                          <td>{db_book.Categories.Where(q => q.IdCategorie == item.Categories).FirstOrDefault().Name}</td>\r\n                    {"<td>" + db_book.KonyvPeldany.Count(q => q.book_id == item.ISBN) + " / " + db_book.KonyvPeldany.Where(q => q.book_id == item.ISBN).Count(q => !q.isBorrowed) + "</td>"}    \r\n                </tr> ";

        }

        //input search = isbn / bookname / writer / categories
        //output {"item":[{"k":{"IdKonyv":,"name":"","writerID":,"authorId":,
            //"ISBN":"","Categories":,"Descrpition":,"imageID":,"demo":,"Available_Quantity":}
            //,"Name":"","writer_name":""}],"pages":1}
        [HttpGet]
        public ActionResult Book(string search = "",int page = 1)
        {
            var item = from k in db_book.konyv
                       join c in db_book.Categories on k.Categories equals c.IdCategorie
                       join w in db_book.Writer on k.authorId equals w.IdWriter
                       select new
                       {
                           k,
                           c.Name,
                           w.writer_name
                       };
            
            if (search.Equals(string.Empty)) return Json(item.OrderBy(q=>q.k.IdKonyv).Skip(30 * (page - 1)).Take(30), JsonRequestBehavior.AllowGet);
            string[] searchsplit = search.ToLower().Split('/');
            if(searchsplit.Length != 4)
            {
                Session["error"] = " LiveSearchBook: input is not formated correctly, it should be ' isbn / bookname / writer / categories '";
            }
            for (int i = 0; i < 4; i++)
            {
                searchsplit[i] = searchsplit[i].Trim();
                if (searchsplit[i].Length < 1) searchsplit[i] = "";

            }
            string searchisbn = searchsplit[0];
            string searchedbook = searchsplit[1];
            string searchedwriter = searchsplit[2];
            string searchedcategories = searchsplit[3];
            item = item.Where(q => q.k.name.Contains(searchedbook) 
            && q.writer_name.Contains(searchedwriter)
            && q.Name.Contains(searchedcategories));
            int pages = (int) Math.Ceiling((double)item.Count() / 30);
            item = item.OrderBy(q => q.k.IdKonyv).Skip(30 * (page - 1)).Take(30);
            var value = new { item , pages };
            return Json(value, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult Rent(string search = "",int page = 1)
        {
            search = search.Trim();
            var rents = db_book.Rent.ToList();
            var item = from rent in rents
                       let bookIdParts = rent.Book_ID.Split(';')
                       let bookId = bookIdParts[0]
                       join konyv in db_book.konyv on bookId equals konyv.ISBN
                       join card in db_book.Reader_Card on rent.Card_ID equals card.IdReaderCard
                       select new
                       {
                           rent,
                           konyv.name,
                           card.Personel_ID_Card
                       };
            if (search.Length > 0) item = item.Where(q => q.Personel_ID_Card.Contains(search));
            int pages = 1;// (int) Math.Ceiling((double)item.Count() / 30);
            item = item.OrderBy(q => q.rent.Rent_Date).Skip(30 * (page - 1)).Take(30);
            var value = new
            {
                item,
                pages
            };
            return Json(value,JsonRequestBehavior.AllowGet);
        }

        public string AllBook()
        {
            string html_code = "";
            //todo create pagenation 25
            foreach (var item in db_book.konyv)
            {
                html_code += RenderBook(item);
            }
            if (html_code.Length < 3) { html_code = "A keresett könyv nincs meg nálunk"; }
            return html_code;
        }
        public string GetReaderCard()
        {
            string curuser = Session["currentreadercard"].ToString();

            if (db_book.Reader_Card.Count(q => q.Personel_ID_Card.Equals(curuser)) > 0)
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
        [HttpGet]
        public ActionResult GetLogBook(int? page)
        {
            int currentpage = 1;
            //db_book.Log.Skip(30 * (currentpage - 1)).Take(30).ToArray();
            var logs = from logdata in db_book.Log join logtext in db_book.LogDetail on logdata.what equals logtext.IdLogDetail.ToString()
                       select new
                       {
                           logdata,
                           logtext
                       };
            if (page.HasValue) currentpage = page.Value;
            logs = logs.OrderBy(q=>q.logdata.IdLog).Skip(30 * (currentpage - 1)).Take(30);
            return Json(logs,JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> LogInUser(string Uname,string Upp,string RegYet)
        {
            Session["success"] = "";
            Session["error"] = "";
            bool succesfullogin = false;
            foreach (var item in db_book.User_sus)
                {
                    if (item.Username.ToLower() == Uname.ToLower() || item.email.ToLower() == Uname.ToLower())
                        {
                    
                        if (item.Userpassword == ComputeStringToSha256Hash(Upp))
                            {
                            Session["username"] = Uname;
                            Session["usermail"] = item.email;
                            Session["userid"] = item.Id;
                            Session["level"] =  db_book.user.First(q=>q.user_id==item.Id).admin;                       
                            Session["success"] = db_book.MessagesSucces.First(q => q.Id == 4).Message;
                            Session["current_page"] = 1;
                            succesfullogin = true;
                            
                            break;
                            }
                        }
        }
            if (succesfullogin)
            {
                await Log("0", Session["userid"].ToString());
                return await IndexPage();
            }

            else
            {
                Session["error"] = db_book.MessagesError.First(q => q.Id == 3).message;
                return await LogInPage();
            }
        }


        public async Task Log(string what)
        {
            Log data = new Log();
            data.who = int.Parse(Session["userid"].ToString());
            data.what = what;
            data.when = DateTime.Now;
            data.whom = "--";
            db_book.Log.Add(data);
          await SaveDatabaseBook();
        }
        public async Task Log(int id, string what)
        {
            Log data = new Log();
            data.who = id;
            data.what = what;
            data.when = DateTime.Now;
            data.whom = "--";
            db_book.Log.Add(data);
           await SaveDatabaseBook();

        }
        public async Task Log(int id,string what,string whom)
        {
            Log data = new Log();
            data.who = id;
            data.what = what;
            data.when = DateTime.Now;
            data.whom = whom;
            db_book.Log.Add(data);
           await SaveDatabaseBook();
        }
        public async Task Log(string what, string whom)
        {
            Log data = new Log();
            data.who = int.Parse(Session["userid"].ToString());
            data.what = what;
            data.when = DateTime.Now;
            data.whom = whom;
            db_book.Log.Add(data);
           await SaveDatabaseBook();
        }
        private string ComputeStringToSha256Hash(string plainText)
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
        protected async Task<int> SaveDatabaseBook()
        {
                await db_book.SaveChangesAsync();
                return 0;
           
        }
    }
}




