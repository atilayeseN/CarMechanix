using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using OilContRM.Models;
using PagedList.Mvc;
using PagedList;
using System.Threading;
using System.Globalization;

namespace OilContRM.Controllers
{
    public class MusteriController : Controller
    {
        
        fuelrmEntities db = new fuelrmEntities();
        
        public ActionResult Index(string lang="en")
        {

            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "YoneticiGiris");
            }
            else {
                if (Session["lang"] == null)
                {
                    Session["lang"] = "en";
                }
                else
                {
                    Session["lang"] = lang;
                }

                Thread.CurrentThread.CurrentCulture = new CultureInfo(Session["lang"].ToString());
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(Session["lang"].ToString());
                if (Session["lang"] == null)
                {
                    Session["lang"] = "en";
                }
                Thread.CurrentThread.CurrentCulture = new CultureInfo(Session["lang"].ToString());
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(Session["lang"].ToString());
                var count = db.Users.Count();
            ViewBag.Count = count;
                var count1 = db.ProcessINFO.Count();
                ViewBag.Processes = count1;

                var time = DateTime.Now.AddDays(-180);
                var procquery = (from proc in db.ProcessContent
                                 join procj in db.ProcessINFO on proc.ID equals procj.ContentID
                                 join car in db.CarInfo on procj.CarID equals car.ID
                                 join user in db.Users on car.UserID equals user.ID
                                 where procj.Time < time && procj.IsCalled == false && user.NeverCall == false && car.IsDelete ==false 
                                 select new GetRecords
                                 {
                                     UserName=user.Name,
                                     Telephone = user.Telephone
                                   




                                 }).ToList();

                ViewBag.Call = procquery.Count();
                return View(procquery);
            }
        }
        public ActionResult CusList(string search="",int page = 1)
        {
            
            
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "YoneticiGiris");
            }
            else
            {

                if (Session["lang"] == null)
                {
                    Session["lang"] = "en";
                }
               
                Thread.CurrentThread.CurrentCulture = new CultureInfo(Session["lang"].ToString());
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(Session["lang"].ToString());
                fuelrmEntities db = new fuelrmEntities();
                var q = (from users in db.Users
                         where users.IsDeleted == false
                         select users).ToList();

               
                return View(q.Where(x => x.Name.Contains(search) || search == null).ToList().OrderBy(m=>m.Name).ToPagedList(page,5));
                
            }
            

        }
        public ActionResult CarList(int id, string search = "")
        {
            if (Session["lang"] == null)
            {
                Session["lang"] = "en";
            }
            Thread.CurrentThread.CurrentCulture = new CultureInfo(Session["lang"].ToString());
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Session["lang"].ToString());
            var a = db.Users.Find(id);
            var b = (from car in db.CarInfo
                    where car.UserID == a.ID && car.IsDelete==false
                    select car).ToList();

            ViewBag.ID = a.ID;
            return View(b.Where(x => x.LicencePlate.Contains(search) || search == null).ToList());
        }
        
        public ActionResult AddCustomerFromModal()
        {
            if (Session["lang"] == null)
            {
                Session["lang"] = "en";
            }
            Thread.CurrentThread.CurrentCulture = new CultureInfo(Session["lang"].ToString());
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Session["lang"].ToString());
            return PartialView();
        }
        [HttpPost]
        public ActionResult AddCustomerFromModal(FormCollection form)
        {
            if (Session["lang"] == null)
            {
                Session["lang"] = "en";
            }
            Thread.CurrentThread.CurrentCulture = new CultureInfo(Session["lang"].ToString());
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Session["lang"].ToString());
            Users user = new Users();
            user.Name = form["name"];
            user.Surname = form["surname"];

            user.Telephone = form["tel"];
            var qtel = form["tel"];
            bool quer = (from i in db.Users
                         where i.Telephone == qtel
                         select i.Telephone).Any();
            if (quer == true)
            {
                return View();
            }
            user.Email = form["email"];
            user.IsDeleted = false;
            user.NeverCall = false;

            db.Users.Add(user);
            db.SaveChanges();
            return RedirectToAction("CusList");
        }
       
        [HttpGet]
        public ActionResult AddCarFromModal(int id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "YoneticiGiris");
            }
            else
            {
                if (Session["lang"] == null)
                {
                    Session["lang"] = "en";
                }
                Thread.CurrentThread.CurrentCulture = new CultureInfo(Session["lang"].ToString());
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(Session["lang"].ToString());

                TempData["id"] = id;



                return PartialView();
            }
        }
        [HttpPost]
        public ActionResult AddCarFromModal(CarModel car)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "YoneticiGiris");
            }
            else
            {
                CarInfo cr = new CarInfo();
                if (car.licence == null || car.branch == null || car.model == null || Convert.ToString(car.age) == null || car.color == null || car.foil == null)
                {
                    var alert = "You have to fill all blanks";
                    ViewBag.Message = alert;
                    return View();
                }
                else
                {

                    cr.LicencePlate = car.licence;
                    cr.UserID = Convert.ToInt32(TempData["id"]);
                    cr.CarBranch = car.branch;
                    cr.Model = car.model;
                    cr.Age = car.age;
                    cr.Color = car.color;
                    cr.OilDate = car.foil;
                    cr.IsDelete = false;

                    var q = db.CarInfo.Where(x => x.LicencePlate.Equals(cr.LicencePlate)).FirstOrDefault();

                    if (q != null)
                    {
                        db.CarInfo.Add(cr);
                        q.IsDelete = true;
                        db.SaveChanges();
                        return RedirectToAction("CarList", new { id = Convert.ToInt32(TempData["id"]) });
                    }
                    else
                    {
                        cr.IsDelete = false;
                        db.CarInfo.Add(cr);
                        db.SaveChanges();
                        return RedirectToAction("CarList", new { id = Convert.ToInt32(TempData["id"]) });
                    }

                }
            }
        }


        public ActionResult Edit ()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "YoneticiGiris");
            }
            else {
                if (Session["lang"] == null)
                {
                    Session["lang"] = "en";
                }
                Thread.CurrentThread.CurrentCulture = new CultureInfo(Session["lang"].ToString());
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(Session["lang"].ToString());
                fuelrmEntities db = new fuelrmEntities();
           
                UserModel u = new UserModel();
           
                return View(u);
            }
        }
       [HttpPost]
        public ActionResult Edit(UserModel data)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "YoneticiGiris");
            }
            else { 
            var us = db.Users.Find(data.id);
          
            if (data.name==null || data.surname==null || Convert.ToString(data.tel)==null)
            {
                    var alert = "You have fill all blanks";
                    ViewBag.Message = alert;
                    return View();
            }
            else { 
            us.Name = data.name;
            us.Surname = data.surname;
            us.Telephone = data.tel;
                db.SaveChanges();
                return RedirectToAction("CusList");
            }
            }
        }
       
        public ActionResult EditCar(int id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "YoneticiGiris");
            }
            else
            {
                if (Session["lang"] == null)
                {
                    Session["lang"] = "en";
                }
                Thread.CurrentThread.CurrentCulture = new CultureInfo(Session["lang"].ToString());
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(Session["lang"].ToString());
                fuelrmEntities db = new fuelrmEntities();
                var a = db.CarInfo.Find(id);
                TempData["id"] = a.ID;
                ViewBag.CarPlate = a.LicencePlate;
                ViewBag.Branch = a.CarBranch;

                CarModel u = new CarModel();

                return View(u);
            }
        }
        [HttpPost]
        public ActionResult EditCar(CarModel data)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "YoneticiGiris");
            }
            else
            {
                if (Session["lang"] == null)
                {
                    Session["lang"] = "en";
                }
                if (data.licence == null || data.branch==null||data.model==null||Convert.ToString(data.age)==null||data.color==null||data.foil==null)
                {
                    var alert = "You have fill all blanks";
                    ViewBag.Message = alert;
                    return View();
                }
                else
                {
                    var car = db.CarInfo.Find(TempData["id"]);
                    car.LicencePlate = data.licence;
                    car.CarBranch = data.branch;
                    car.Model = data.model;
                    car.Age = data.age;
                    car.Color = data.color;
                    car.IsDelete = data.delete;
                    car.OilDate = data.foil;
                    
                    
                    
                    
                    db.SaveChanges();
                    return RedirectToAction("CarList",new { id = car.UserID });
                }
            }
        }

        public ActionResult DeleteCar(int id)
        {
            var car = db.CarInfo.Find(id);
            var us = db.Users.Find(car.UserID);
            car.IsDelete = true;
            db.SaveChanges();

            return RedirectToAction("CarList", new { id = us.ID });
        }
        public ActionResult DeleteUser(int? id)
        {
           if(id != null)
            {
                var a=db.Users.Find(id);
                var b = (from car in db.CarInfo
                         where car.UserID == a.ID
                         select car).ToList();
                foreach(var infs in b)
                {
                    infs.IsDelete = true;
                }
                a.IsDeleted = true;
                db.SaveChanges();
            }

            return RedirectToAction("CusList");
         }
        [HttpGet]
        public ActionResult SendEmail(int id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "YoneticiGiris");
            }
            else {
                if (Session["lang"] == null)
                {
                    Session["lang"] = "en";
                }
                Thread.CurrentThread.CurrentCulture = new CultureInfo(Session["lang"].ToString());
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(Session["lang"].ToString());
                fuelrmEntities db = new fuelrmEntities();
            var name = db.Users.Find(id);
            ViewBag.Name = name.Name;
            ViewBag.ID = id;

            return View();
            }
        }
        [HttpPost]
        public ActionResult SendEmail(FormCollection form)
        {
            fuelrmEntities db = new fuelrmEntities();
            string adsubject = "RepairInformation";
            string admmessage = form["message"];
            var id = Convert.ToInt32(form["id"]);
            var receiver = (from i in db.Users
                            where i.ID == id
                            select i.Email).FirstOrDefault();



            string subject = adsubject;
            string message = admmessage;
            var senderEmail = new MailAddress("anqellofhells@gmail.com", "Atilay");
            var receiverEmail = new MailAddress(receiver, "Customer");
            var password = "98atilay";
            var sub = subject;
            var body = message;
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(senderEmail.Address, password)
            };
            using (var mess = new MailMessage(senderEmail, receiverEmail)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(mess);
            }
            return RedirectToAction("CusList");


        }
        public ActionResult Contact()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "YoneticiGiris");
            }
            else
            {
                if (Session["lang"] == null)
                {
                    Session["lang"] = "en";
                }
                Thread.CurrentThread.CurrentCulture = new CultureInfo(Session["lang"].ToString());
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(Session["lang"].ToString());
                var time = DateTime.Now.AddDays(-180);
            var procquery = (from proc in db.ProcessContent
                             join procj in db.ProcessINFO on proc.ID equals procj.ContentID
                             join car in db.CarInfo on procj.CarID equals car.ID
                             join user in db.Users on car.UserID equals user.ID
                             where procj.Time < time && procj.IsCalled==false && user.NeverCall == false && car.IsDelete == false
                             select new GetRecords
                             {
                                 CarID = car.ID,
                                 Time = procj.Time,
                                 LicencePlate = car.LicencePlate,
                                 Description = procj.Description,
                                 Explain = proc.Description,
                                 ProcID = procj.ID,
                                 Call=procj.IsCalled,
                                 UserName=user.Name
                                 




                             }).ToList();
            if (procquery.Count() == 0)
            {
                ViewBag.Not = "Boş";
            }

            return View(procquery);
            }
        }
        public ActionResult Called (int id,int? nevercall)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "YoneticiGiris");
            }
            else { 
            var process = db.ProcessINFO.Find(id);
            process.IsCalled = true;
            CallRecord call = new CallRecord();
            call.ProcessID = id;
            call.Time = DateTime.Now;
            var car = (from i in db.ProcessINFO
                       where i.ID == id
                       select i.CarID).FirstOrDefault();
            var userID = (from a in db.CarInfo
                          where a.ID == car
                          select a.UserID).FirstOrDefault();
            var us = db.Users.Find(userID);
            var user =us.Name;
            
            call.CustomerName = user;
            call.Car = db.CarInfo.Find(car).CarBranch;
            call.LicencePlate = db.CarInfo.Find(car).LicencePlate;
            call.Telephone = db.Users.Find(userID).Telephone;
            db.CallRecord.Add(call);
            if(nevercall != null)
            {
                us.NeverCall = true;
            }
            db.SaveChanges();
            

            return RedirectToAction("Contact");
            }
        }
        public ActionResult CallRecords(int page=1,string search="")
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "YoneticiGiris");
            }
            else { 
            if (Session["lang"] == null)
            {
                Session["lang"] = "en";
            }
            Thread.CurrentThread.CurrentCulture = new CultureInfo(Session["lang"].ToString());
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Session["lang"].ToString());
            var calls = db.CallRecord.ToList();
            return View(calls.Where(x => x.LicencePlate.Contains(search) || search == null).ToList().OrderByDescending(m=>m.Time).ToPagedList(page,5));
            }


        }

      
 
        
        






    }
}