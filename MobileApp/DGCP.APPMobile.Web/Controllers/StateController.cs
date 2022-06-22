using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DGCP.APPMobile.Web.Services;

namespace DGCP.APPMobile.Web.Controllers
{
    public class StateController : ApiController
    {
        private StateService _sService;
        public StateController()
        {
            _sService = new StateService();
        }

        [ActionName("List")]
        public HttpResponseMessage GetList(int page = 1, [FromUri]List<string> selected = null, string searchCriteria = "")
        {
            var result = new HttpResponseMessage();

            try
            {
                var states = _sService.GetStates(page, selected, searchCriteria);

                result = Request.CreateResponse(HttpStatusCode.OK, states);
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
