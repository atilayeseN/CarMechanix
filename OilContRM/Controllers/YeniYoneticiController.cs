using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OilContRM.Models;

namespace OilContRM.Controllers
{
    public class YeniYoneticiController : Controller
    {
        [AllowAnonymous]
        public ActionResult Submit()
        {
            return View();
        }
        
        [HttpPost]
        
        public ActionResult Submit(FormCollection form)
        {
           
            fuelrmEntities db = new fuelrmEntities();
            Admins admin = new Admins();
            admin.Username = form["uname"];
            if (db.Admins.Any(x => x.Username == admin.Username))
            {
                var alert = "Username is already exist";
                ViewBag.Message = alert;
                return View();

            }
            admin.Password = form["pass"];
           
            admin.Email = form["email"];
            if (db.Admins.Any(x => x.Email == admin.Email))
            {
                var alert = "Email is already exist";
                ViewBag.EMessage = alert;
                return View();
            }
            db.Admins.Add(admin);
            db.SaveChanges();

            return RedirectToAction("Index", "YoneticiGiris");
        }
    }
}