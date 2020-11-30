using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OilContRM.Models
{
    public class GetRecords
    {
        public int CarID { get; set; }
        public string Branch { get; set; }
        public string Model   { get; set; }
        public string Color { get; set; }
        public string LicencePlate { get; set; }
        public DateTime? Time { get; set; }
        public string Description { get; set; }
        public string Explain { get; set; }
        public int ProcID { get; set; }
        public bool? Call { get; set; }
        public string UserName { get; set; }
        public string Telephone { get; set; }
        public int pageSize { get; set; }
        public int pageNumber { get; set; }
        public bool? NeverCall { get; set; }

    }
}