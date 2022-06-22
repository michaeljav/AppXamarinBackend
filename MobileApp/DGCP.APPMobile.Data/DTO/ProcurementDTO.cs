using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGCP.APPMobile.Data.DTO
{
    public class ProcurementDTO
    {
        public string PublicationId { get; set; }
        public string Period { get; set; }
        public PurchasingUnitDTO PurchasingUnit { get; set; }
        public string ProcessId { get; set; }
        public string Description { get; set; }
        public DateTime? ReceptionEndDate { get; set; }
        //public DateTime? ApprovalDate { get; set; }
        public string Link { get; set; }
        public string Status { get; set; }
        public SectorDTO Sector { get; set; }
        public ContactDTO Contact { get; set; }
    }
}
