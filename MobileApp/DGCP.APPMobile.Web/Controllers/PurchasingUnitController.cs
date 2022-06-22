using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DGCP.APPMobile.Web.Services;

namespace DGCP.APPMobile.Web.Controllers
{
    public class PurchasingUnitController : ApiController
    {
        public PurchasingUnitService _pUnitService;
        public PurchasingUnitController()
        {
            _pUnitService = new PurchasingUnitService();
        }
        // GET api/purchasingunit
         [ActionName("List")]
        public HttpResponseMessage GetList(int page = 1, [FromUri]List<string> ministrySelected = null, [FromUri]List<string> selected = null, string searchCriteria = "")
        {
            var result = new HttpResponseMessage();

            try
            {
                var purchasingUnit = _pUnitService.GetPurchasingUnits(page, ministrySelected, selected, searchCriteria);

                result = Request.CreateResponse(HttpStatusCode.OK, purchasingUnit);
            }
            catch (Exception e)
            {
                result = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return result;
        }

        // GET api/purchasingunit/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/purchasingunit
        public void Post([FromBody]string value)
        {
        }

        // PUT api/purchasingunit/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/purchasingunit/5
        public void Delete(int id)
        {
        }
    }
}
