using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DGCP.APPMobile.Web.Services;

namespace DGCP.APPMobile.Web.Controllers
{
    public class SectorController : ApiController
    {
        private SectorService _sService;
        public SectorController()
        {
            _sService = new SectorService();
        }

        [ActionName("List")]
        public HttpResponseMessage GetList(int page = 1, [FromUri]List<string> selected = null, string searchCriteria = "")
        {
            var result = new HttpResponseMessage();

            try
            {
                var sectors = _sService.GetSectors(page, selected, searchCriteria);

                result = Request.CreateResponse(HttpStatusCode.OK, sectors);
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
