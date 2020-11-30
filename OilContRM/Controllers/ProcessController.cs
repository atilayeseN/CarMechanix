using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using OilContRM.Models;
using PagedList.Mvc;
using PagedList;
using System.Globalization;
using System.Threading;

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
                if (Session["lang"] == null)
                {
                    Session["lang"] = "en";
                }
                Thread.CurrentThread.CurrentCulture = new CultureInfo(Session["lang"].ToString());
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(Session["lang"].ToString());
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
            process.IsCalled = false;
            db.ProcessINFO.Add(process);
            db.SaveChanges();


            




            return RedirectToAction("TakeProcess", new { id = cid });
            }
            // return RedirectToAction("CarList","Musteri", new { id = query});
        }


        [HttpGet]
        public ActionResult ShowProcess(string search,int page=1)
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
            
            var procquery = (from proc in db.ProcessContent
                             join procj in db.ProcessINFO on proc.ID equals procj.ContentID
                             join car in db.CarInfo on procj.CarID equals car.ID
                             join user in db.Users on car.UserID equals user.ID
                             select new GetRecords
                             {
                        CarID = car.ID,
                        Branch = car.CarBranch,
                        Model = car.Model,
                        Color = car.Color,
                        Time=procj.Time,
                        LicencePlate=car.LicencePlate,
                        Description=procj.Description,
                        Explain=proc.Description,
                        ProcID=procj.ID,
                        UserName=user.Name
                        
                       


                    });


          
              
        
               

            return View(procquery.Where(x => x.LicencePlate.Contains(search) || search == null).ToList().OrderBy(m=>m.LicencePlate).ToPagedList(page,5));
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
        [HttpGet]
        public ActionResult Explain(int id)
        {
            fuelrmEntities db = new fuelrmEntities();
            var process = db.ProcessINFO.Find(id);
            var contid = process.ContentID;
            var content = db.ProcessContent.Find(contid);

           
            return PartialView(content);
        }
        public ActionResult AddProcessFromModal()
        {
            if (Session["lang"] == null)
            {
                Session["lang"] = "en";
            }
            Thread.CurrentThread.CurrentCulture = new CultureInfo(Session["lang"].ToString());
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Session["lang"].ToString());
            fuelrmEntities db = new fuelrmEntities();
            List<SelectListItem> plates = (from i in db.CarInfo.ToList()
                                           where i.IsDelete == false
                                          select new SelectListItem
                                          {
                                              Text = i.LicencePlate,
                                              Value = i.ID.ToString()
                                          }).ToList();
            List<SelectListItem> products = (from i in db.URUNLISTEM.ToList()
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
            ViewBag.Licence = plates;
            ViewBag.Products = products;
            ViewBag.Types = types;

            return PartialView();
        }
        [HttpPost]
        public ActionResult AddProcessFromModal(ModalData records)
        {
            fuelrmEntities db = new fuelrmEntities();

            ProcessContent content = new ProcessContent();
            content.Type = records.type;
            content.Description = records.details;
            content.ProuctID = records.product;
            var prodname = (from i in db.URUNLISTEM
                            where i.PRODUCT_CODE == records.product.ToString()
                            select i.PRODUCT_NAME).FirstOrDefault();
            content.ProductName = prodname;
            db.ProcessContent.Add(content);
            ProcessINFO info = new ProcessINFO();
            info.Description = records.description;
            info.ContentID = content.ID;
            info.CarID = Convert.ToInt32(records.licence);
            info.Time = DateTime.Now;
            info.AdminID = Convert.ToInt32(Session["UserID"]);
            info.IsCalled = false;
            db.ProcessINFO.Add(info);
            db.SaveChanges();



            return RedirectToAction("ShowProcess");
        }
       









    }
    
}