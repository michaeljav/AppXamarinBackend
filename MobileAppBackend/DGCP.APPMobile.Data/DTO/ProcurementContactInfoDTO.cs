using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGCP.APPMobile.Data.DTO
{
    public class ProcurementContactInfoDTO
    {
        public string PublicationId { get; set; }
        public string Period { get; set; }
        //public PurchasingUnitDTO PurchasingUnit { get; set; }
        public ContactDTO Contact { get; set; }
    }
}
