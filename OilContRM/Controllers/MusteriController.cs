using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using OilContRM.Models;

namespace OilContRM.Controllers
{
    public class MusteriController : Controller
    {
        
        fuelrmEntities db = new fuelrmEntities();
        
        public ActionResult Index()
        {
            
            
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "YoneticiGiris");
            }
            else { 
            var count = db.Users.Count();
            ViewBag.Count = count;
                var count1 = db.ProcessINFO.Count();
                ViewBag.Processes = count1;


            
            
            return View();
            }
        }
        public ActionResult CusList()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "YoneticiGiris");
            }
            else {
                fuelrmEntities db = new fuelrmEntities();
                var q = (from users in db.Users
                         where users.IsDeleted == false
                         select users).ToList();
              
                    

               
                return View(q);
            }
            

        }
        public ActionResult CarList(int id)
        {
            var a = db.Users.Find(id);
            var b = (from car in db.CarInfo
                    where car.UserID == a.ID && car.IsDelete==false
                    select car).ToList();

            ViewBag.ID = a.ID;
            return View(b);
        }
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Add(FormCollection form)
        {
            if(Session["UserID"] == null)
            {
                return RedirectToAction("Index", "YoneticiGiris");
            }
            else
            {
                
                fuelrmEntities db = new fuelrmEntities();
                string name = form["name"];
                string surname = form["surname"];
                var q = (from us in db.Users
                         where us.Name == name && us.Surname == surname
                         select us).Any();
                if (q == true)
                {
                    var exus = (from uss in db.Users
                                where uss.Name == name && uss.Surname == surname
                                select uss).FirstOrDefault();
                    var excars = (from carss in db.CarInfo
                                  where carss.UserID == exus.ID
                                  select carss).ToList();
                    foreach(var i in excars)
                    {
                        i.IsDelete = false;
                    }
                    exus.IsDeleted = false;
                    db.SaveChanges();
                }
                
              
                else {
                Users user = new Users();
                user.Name = form["name"];
                user.Surname = form["surname"];
                user.Telephone = form["tel"];
                user.Email = form["email"];
                user.IsDeleted = false;
              
                db.Users.Add(user);
                db.SaveChanges();
                }


                return RedirectToAction("CusList");






            }
           
        }
        
        public ActionResult Edit ()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "YoneticiGiris");
            }
            else { 
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
        public ActionResult AddCar(int id)
        {
            var a = db.Users.Find(id);
            TempData["userid"] = a.ID;

            

            return View();
        }
        [HttpPost]
        public ActionResult AddCar(CarModel car)
        {
            CarInfo cr = new CarInfo();
            if(car.licence==null||car.branch==null||car.model == null|| Convert.ToString(car.age) == null || car.color == null || car.foil == null)
            {
                var alert = "You have to fill all blanks";
                ViewBag.Message = alert;
                return View();
            }
            else { 
            cr.UserID = Convert.ToInt32(TempData["userid"]);
            cr.LicencePlate = car.licence;
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
                return RedirectToAction("CarList", new { id = Convert.ToInt32(TempData["userid"]) });
            }
            else
            {
                cr.IsDelete = false;
                db.CarInfo.Add(cr);
                db.SaveChanges();
                return RedirectToAction("CarList", new { id = Convert.ToInt32(TempData["userid"]) });
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
                fuelrmEntities db = new fuelrmEntities();
                var a = db.CarInfo.Find(id);
                TempData["id"] = a.ID;

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
            fuelrmEntities db = new fuelrmEntities();
            var name = db.Users.Find(id);
            ViewBag.Name = name.Name;
            ViewBag.ID = id;

            return View();
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






    }
}