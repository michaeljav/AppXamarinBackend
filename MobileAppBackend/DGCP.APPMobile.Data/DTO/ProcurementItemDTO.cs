using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGCP.APPMobile.Data.DTO
{
    public class ProcurementItemDTO
    {
        public string PublicationId { get; set; }
        //public PurchasingUnitDTO PurchasingUnit { get; set; }
        public string Period { get; set; }
        public string ItemId { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public string DesUNSPSC { get; set; }


        

    }
}
