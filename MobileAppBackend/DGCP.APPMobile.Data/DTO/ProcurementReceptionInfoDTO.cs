using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGCP.APPMobile.Data.DTO
{
    public class ProcurementReceptionInfoDTO
    {
        public string PublicationId { get; set; }
        public string Period { get; set; }
        //public PurchasingUnitDTO PurchasingUnit { get; set; }
        //public DateTime? ReceptionDate { get; set; }
        public DateTime? StartOffersReceptionDate { get; set; }
        public DateTime? EndOffersReceptionDate { get; set; }
        public DateTime? ExtendedOffersReceptionDate { get; set; }
        public string OfferAddress { get; set; }
        public DateTime? FirstOpeningDate { get; set; }
        public string FirstOpeningHour { get; set; }
        public DateTime? FirstExtendedOpeningDate { get; set; }
        public string FirstExtendedOpeningHour { get; set; }
        public DateTime? SecondOpeningDate { get; set; }
        public string SecondOpeningHour { get; set; }
        public DateTime? SecondExtendedOpeningDate { get; set; }
        public string SecondExtendedOpeningHour { get; set; }
        public string OpeningPlaceAddress { get; set; }
        //public DateTime? ApprovalDate { get; set; }
    }
}
