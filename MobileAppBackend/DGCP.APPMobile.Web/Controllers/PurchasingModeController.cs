using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DGCP.APPMobile.Web.Services;

namespace DGCP.APPMobile.Web.Controllers
{
    public class PurchasingModeController : ApiController
    {
        private PurchasingModeService _pMService;
        public PurchasingModeController()
        {
            _pMService = new PurchasingModeService();
        }

        [ActionName("List")]
        public HttpResponseMessage GetList(int page = 1, [FromUri]List<string> selected = null, string searchCriteria = "")
        {
            var result = new HttpResponseMessage();

            try
            {
                var purchasingModes = _pMService.GetPurchasingModes(page, selected, searchCriteria);

                result = Request.CreateResponse(HttpStatusCode.OK, purchasingModes);
            }
            catch (Exception e)
            {
                result = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return result;
        }

        // POST api/purchasingmode
        public void Post([FromBody]string value)
        {
        }

        // PUT api/purchasingmode/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/purchasingmode/5
        public void Delete(int id)
        {
        }
    }
}
