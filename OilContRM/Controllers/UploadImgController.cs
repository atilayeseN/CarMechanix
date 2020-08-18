using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using System.Web.Helpers;
using System.Drawing.Imaging;
using OilContRM.Models;

namespace OilContRM.Controllers
{
    public class UploadImgController : Controller
    {
        // GET: UploadImg
        public ActionResult Index()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "YoneticiGiris");
            }
            else
            {

                return View();
            }
            

        }
        [HttpPost]
        public ActionResult ImageUpload(HttpPostedFileBase uploadfile)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "YoneticiGiris");
            }
            else
            {
                string path = System.Web.HttpContext.Current.Server.MapPath("~/Content/images").ToString();

                if (uploadfile == null)
                {
                    return RedirectToAction("Index");

                }


                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                if (uploadfile.ContentLength > 100)
                {
                    string TempFile = Path.Combine(path.ToString() + "\\" + Path.GetFileName("_TempLOGO.jpg"));
                    string IMGFile = Path.Combine(path.ToString() + "\\" + Path.GetFileName("_LOGO.jpg"));

                    System.IO.File.Delete(IMGFile);

                    uploadfile.SaveAs(TempFile);



                    using (Bitmap bmp = (Bitmap)Image.FromFile(TempFile))
                    {
                        int maxw = 45;
                        int maxh = 45;


                        double ratiox = (double)(maxw) / bmp.Width;
                        var ratioy = (double)(maxh) / bmp.Height;
                        var ratio = Math.Min(ratiox, ratioy);
                        var neww = (int)(bmp.Width * ratio);
                        var newh = (int)(bmp.Height * ratio);


                        using (var newIMG = new Bitmap(neww, newh))
                        {
                            using (var graphics = Graphics.FromImage(newIMG))
                                graphics.DrawImage(bmp, 0, 0, neww, newh);
                            newIMG.Save(IMGFile);

                        }
                    }






                    System.IO.File.Delete(TempFile);


                }
                return RedirectToAction("Index", "Musteri");
            }
        }
     

    }




    }
