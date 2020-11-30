using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using OilContRM.Models;

namespace OilContRM.Controllers
{
    public class ProcessController : Controller
    {
        // GET: Process
       



        
        public ActionResult TakeProcess(int id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "YoneticiGiris");
            }
            else { 
                fuelrmEntities db = new fuelrmEntities();
            var carname = (from n in db.CarInfo
                           where n.ID == id
                           select n).FirstOrDefault();
                List<SelectListItem> prods = (from i in db.URUNLISTEM.ToList()
                                              select new SelectListItem
                                              {
                                                  Text = i.PRODUCT_NAME,
                                                  Value = i.PRODUCT_CODE.ToString()
                                              }).ToList();
                List<SelectListItem> types = (from i in db.URUNLISTEM.ToList()
                                              select new SelectListItem
                                              {
                                                  Text = i.PRODUCT_TYPE,
                                                  Value = i.PRODUCT_CODE.ToString()
                                              }).ToList();
                ViewBag.Types = types;
                ViewBag.Products = prods;
            ViewBag.name = carname.CarBranch;
            ViewBag.model = carname.Model;
            ViewBag.licence = carname.LicencePlate;
     
            TempData["id"] = id;
            return View();
            }
        }
        [HttpPost]
        public ActionResult AddProcess(ProcessModel p)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "YoneticiGiris");
            }
            else { 
            var cid = Convert.ToInt32(TempData["id"]);
            fuelrmEntities db = new fuelrmEntities();
            ProcessContent content = new ProcessContent();
            content.Description = p.details;
            content.ProuctID = p.productid;
            content.Type = p.type;
                var prodname = (from i in db.URUNLISTEM
                                where i.PRODUCT_CODE == p.productid.ToString()
                                select i
                              ).FirstOrDefault();
                content.ProductName = prodname.PRODUCT_NAME;
            db.ProcessContent.Add(content);
            ProcessINFO process = new ProcessINFO();
            process.ContentID = content.ID;
            process.Description = p.describtion;
            process.Time = DateTime.Now;
            process.AdminID = Convert.ToInt32(Session["UserID"]);
            process.CarID = Convert.ToInt32(cid);
            db.ProcessINFO.Add(process);
            db.SaveChanges();


            var query = (from car in db.CarInfo
                         where car.ID == cid
                         select car.UserID).FirstOrDefault();





            return RedirectToAction("TakeProcess", new { id = cid });
            }
            // return RedirectToAction("CarList","Musteri", new { id = query});
        }


        
        public ActionResult ShowProcess()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "YoneticiGiris");
            }
            else { 
            fuelrmEntities db = new fuelrmEntities();
       
            var procquery = (from proc in db.ProcessContent
                             join procj in db.ProcessINFO on proc.ID equals procj.ContentID
                             join car in db.CarInfo on procj.CarID equals car.ID
                             select new GetRecords
                             {
                        Branch = car.CarBranch,
                        Model = car.Model,
                        Color = car.Color,
                        Time=procj.Time,
                        LicencePlate=car.LicencePlate,
                        Description=procj.Description,
                        Explain=proc.Description,
                        ProcID=procj.ID,
                        
                       


                    }).ToList();
            
            




            return View(procquery);
            }
        }
        public ActionResult DeleteProcess(int id)
        {
            fuelrmEntities db = new fuelrmEntities();
            var pcsi= db.ProcessINFO.Find(id);
            var pcsc = (from cont in db.ProcessContent
                        where cont.ID == pcsi.ContentID
                        select cont).FirstOrDefault();
            var inff = pcsc.ID;
            var info = db.ProcessContent.Find(inff);
            db.ProcessContent.Remove(info);
            db.ProcessINFO.Remove(pcsi);
            db.SaveChanges();
            return RedirectToAction("ShowProcess");
        }
        public ActionResult SendBill(int? id)
        {
            fuelrmEntities db = new fuelrmEntities();
            var process = db.ProcessINFO.Find(id);
            var carid = (from i in db.CarInfo
                         where i.ID == process.CarID
                         select i.ID).FirstOrDefault();
            var car = db.CarInfo.Find(carid);
            var user = db.Users.Find(car.UserID);
            var admin = db.Admins.Find(process.AdminID);
            string adsubject = "Bill";
            string admmessage = "Time: "  +  process.Time  + Environment.NewLine + "Your car is: "+ car.CarBranch + " " + car.Model+ " " + car.Color + " " + car.LicencePlate +" " + Environment.NewLine + "Your process: " + process.Description + Environment.NewLine + "Worker who did your work: " + admin.Username;

            var receiver = user.Email;



            string subject = adsubject;
            string message = admmessage;
            var senderEmail = new MailAddress("anqellofhells@gmail.com", "Atilay");
            var receiverEmail = new MailAddress(receiver, "Customer");
            var password = "98atilay";
            var sub = subject;
            var body = message;
            var smtp = new SmtpClient
            {
                Host = "smtp.office365.com",
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


            return RedirectToAction("ShowProcess");
        }
        public ActionResult ProcessModal(int id)
        {
            fuelrmEntities db = new fuelrmEntities();
            var procinf = db.ProcessINFO.Find(id);
            var proccont = db.ProcessContent.Find(procinf.ContentID);

            return PartialView(proccont);

            
        }
        
     




    }
    
}