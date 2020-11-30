using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OilContRM.Models;
using System.ComponentModel.DataAnnotations; 

namespace OilContRM.Models
{
    public class CarModel
    {
        fuelrmEntities db = new fuelrmEntities();   
        
   
        public int carid { get; set; }
        public string licence { get; set; }
        public string oiltype { get; set; }
        public DateTime foil { get; set; }
        public string branch { get; set; }
        public string model { get; set; }
        public int userid { get; set;}
        public int age { get; set;}
        public string color { get; set; }
        public bool delete { get; set; }

    }
}