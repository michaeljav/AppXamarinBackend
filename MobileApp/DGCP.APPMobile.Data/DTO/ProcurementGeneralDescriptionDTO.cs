using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGCP.APPMobile.Data.DTO
{
    public class ProcurementGeneralDescriptionDTO
    {
        //public string ProcurementId { get; set; }
        public string Period { get; set; }
        public string PublicationId { get; set; }
        public MinistryDTO Ministry { get; set; }
        public PurchasingUnitDTO PurchasingUnit { get; set; }
        public PurchasingModeDTO PurchasingMode { get; set; }
        public string ProcessId { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public SectorDTO Sector { get; set; }
        public DateTime? PublicationDate { get; set; }
        public DateTime? EstimatedAdjudicationDate { get; set; }
        public string StatementId { get; set; }
    }
}
