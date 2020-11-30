using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using OilContRM.Models;

namespace OilContRM.Controllers
{
    public class YoneticiGirisController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Authorize(FormCollection form)
        {

            using (fuelrmEntities db = new fuelrmEntities())
            {
                var a = form["uname"].Trim();
                var b = form["pass"].Trim();

                var v = db.Admins.Where(x => x.Username.Equals(a) && x.Password.Equals(b)).FirstOrDefault();
                if (v == null)
                {
                    return RedirectToAction("Submit", "YeniYonetici");
                }
                else
                {
                    Session["UserID"] = v.ID;
                    return RedirectToAction("Index", "Musteri");
                }
            }
        }
        public ActionResult SendEmail()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SendEmail(FormCollection form)
        {

                fuelrmEntities db = new fuelrmEntities();
                var receiver = form["email"];
            var q = (from i in db.Admins
                     where i.Email == receiver
                     select i.Email).Any();
            if (q == true) { 
                var adminid = (from i in db.Admins
                           where i.Email == receiver
                           select i.ID).FirstOrDefault();
               
                var subject = "REPASSWORD";
                var message = "localhost:55251/YoneticiGiris/RePassword/"+adminid;
                    var senderEmail = new MailAddress("anqellofhells@gmail.com", "Atilay");
                    var receiverEmail = new MailAddress(receiver, "Receiver");
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
                    return View();
            }
            else
            {
                return RedirectToAction("Index");
            }



        }
        public ActionResult RePassword(int id)
        {
            fuelrmEntities db = new fuelrmEntities();
            ViewBag.ID = id;
            
            return View();

        }
        [HttpPost]
        public ActionResult RePassword(FormCollection form)
        {
            var time = DateTime.Now;
            
            
            fuelrmEntities db = new fuelrmEntities();
            
            
            var asc =  (from i in db.Password_LOG
                       orderby i.ID
                       select i.TİME).FirstOrDefault();

            DateTime logabletime = asc.AddMinutes(5);
            if (logabletime<time)
            {
                Password_LOG log = new Password_LOG();
                string g = Guid.NewGuid().ToString();
                log.TİME = DateTime.Now;
                log.GUİD = g;
                db.Password_LOG.Add(log);
                var id = Convert.ToInt32(form["adminid"]);
                var admin = db.Admins.Find(id);
                admin.Guid = g;
                admin.Password = form["newpass"];
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("SendEmail");
            }



            
            
       
           

        }
    }
}