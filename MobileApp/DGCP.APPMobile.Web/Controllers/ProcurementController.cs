using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DGCP.APPMobile.Data.DTO;
using DGCP.APPMobile.Web.Services;

namespace DGCP.APPMobile.Web.Controllers
{
    public class ProcurementController : ApiController
    {
        private ProcurementService _pService;
        public ProcurementController()
        {
            _pService = new ProcurementService();
        }

        [ActionName("GeneralDescription")]
        public HttpResponseMessage GetGeneralDescription(string publicationId, string period)
        {
            var result = new HttpResponseMessage();

            try
            {
                var procurement = _pService.GetProcurementGeneralDescription(publicationId, period);

                result = Request.CreateResponse(HttpStatusCode.OK, procurement);
            }
            catch (Exception e)
            {
                result = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return result;
        }

        [ActionName("ContactInfo")]
        public HttpResponseMessage GetContactInfo(string publicationId, string period)
        {
            var result = new HttpResponseMessage();

            try
            {
                var procurement = _pService.GetProcurementContactInfo(publicationId, period);

                result = Request.CreateResponse(HttpStatusCode.OK, procurement);
            }
            catch (Exception e)
            {
                result = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return result;
        }

        [ActionName("ReceptionInfo")]
        public HttpResponseMessage GetReceptionInfo(string publicationId, string period)
        {
            var result = new HttpResponseMessage();

            try
            {
                var procurement = _pService.GetProcurementReceptionInfo(publicationId, period);

                result = Request.CreateResponse(HttpStatusCode.OK, procurement);
            }
            catch (Exception e)
            {
                result = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return result;
        }

        [ActionName("Info")]
        public HttpResponseMessage GetInfo(string procurementId, string purchasingUnitId)
        {
            var result = new HttpResponseMessage();

            try
            {
                var procurement = _pService.GetProcurementInfo(procurementId, purchasingUnitId);

                result = Request.CreateResponse(HttpStatusCode.OK, procurement);
            }
            catch (Exception e)
            {
                result = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return result;
        }

        // GET api/procurement
        [ActionName("List")]
        public HttpResponseMessage GetList(int page = 1, [FromUri]List<string> purchasingUnitId = null, [FromUri]List<string> ministryId = null,
                                           [FromUri]List<string> purchaseModeId = null, string processId = "", string description = "",
                                           [FromUri]List<string> sectorId = null, [FromUri]List<string> stateId = null, DateTime? publicationStartDate = null,
                                           DateTime? publicationEndDate = null, DateTime? receptionStartDate = null,
                                           DateTime? receptionEndDate = null, bool filterFlag = false, bool miPyMeFlag = false, bool configFlag = false, bool notificationFlag = false)
        {
            
            var result = new HttpResponseMessage();

            try
            {
                var procurements = _pService.GetProcurements(page, purchasingUnitId, ministryId,
                                                             purchaseModeId, processId, description,
                                                             sectorId, stateId, publicationStartDate,
                                                             publicationEndDate, receptionStartDate,
                                                             receptionEndDate, filterFlag, miPyMeFlag, configFlag, notificationFlag);

                result = Request.CreateResponse(HttpStatusCode.OK, procurements);
            }
            catch(Exception e)
            {
                result = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return result;
        }

        [ActionName("Items")]
        public HttpResponseMessage GetItems(string publicationId, string period, int page = 1)
        {
            var result = new HttpResponseMessage();
            try
            {
                var procurement = _pService.GetProcurementItems(publicationId, period, page);
                result = Request.CreateResponse(HttpStatusCode.OK, procurement);


            }
            catch (Exception e)
            {
                result = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return result;
        }

        [ActionName("TinyUrl")]
        public HttpResponseMessage GetTinyUrl(string publicationId, string period)
        {
            var result = new HttpResponseMessage();
            try
            {
                var procurement = _pService.GetProcurementTinyURL(publicationId, period);
                result = Request.CreateResponse(HttpStatusCode.OK, procurement);


            }
            catch (Exception e)
            {
                result = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return result;
        }

        // POST api/procurement
        public void Post([FromBody]string value)
        {
        }

        // PUT api/procurement/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/procurement/5
        public void Delete(int id)
        {
        }
    }
}
