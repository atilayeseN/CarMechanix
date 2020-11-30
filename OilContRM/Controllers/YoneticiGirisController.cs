using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using OilContRM.Models;

namespace OilContRM.Controllers
{
    public class YoneticiGirisController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            if(Request.Cookies["LoginCookie"] == null)
            {
                return View();
            }
            else
            {
                Session["UserID"] = Request.Cookies["LoginCookie"].Value;
                return RedirectToAction("Index", "Musteri");
            }
            
        }
        [HttpPost]
        public ActionResult Authorize(FormCollection form)
        {

            using (fuelrmEntities db = new fuelrmEntities())
            {
                var username = form["uname"].Trim();
                var password = form["pass"].Trim();
                var check = form["rememberme"];

                var v = db.Admins.Where(x => x.Username.Equals(username) && x.Password.Equals(password)).FirstOrDefault();
                if (v == null)
                {
                    return RedirectToAction("Submit", "YeniYonetici");
                }
                else
                {
                    Session["UserID"] = v.ID;
                    if(check != null&& Request.Cookies["LoginCookie"] == null)
                    {
                            HttpCookie cookie = new HttpCookie("LoginCookie", Session["UserID"].ToString());
                            cookie.Expires = DateTime.Now.AddHours(5);
                            Response.Cookies.Add(cookie);
                    }
                        Login_LOG log = new Login_LOG();
                        string g = Guid.NewGuid().ToString();
                        log.Guid = g;
                        v.LogGUID = g;
                        log.Time = DateTime.Now;
                        db.Login_LOG.Add(log);
                        db.SaveChanges();

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
                       orderby i.ID descending
                       select i.TİME).FirstOrDefault().AddMinutes(5);

           
            if (asc<time)
            {

                Password_LOG log = new Password_LOG();
                string g = Guid.NewGuid().ToString();
                log.TİME = DateTime.Now;
                log.GUİD = g;
                db.Password_LOG.Add(log);
                var id = Convert.ToInt32(form.Get("adminid"));
                var admin = db.Admins.Find(id);
                admin.Password = form["newpass"];
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                Session["Error"] = "You cant change your password repeatly";
                return RedirectToAction("Index");
            }



            
            
       
           

        }
        public ActionResult Logout()
        {
            Response.Cookies["LoginCookie"].Expires = DateTime.Now.AddDays(-1);
            Session.Abandon();
            Session.Clear();
            return RedirectToAction("Index","YoneticiGiris"); 
        }
    }
}