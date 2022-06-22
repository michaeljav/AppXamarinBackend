using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DGCP.APPMobile.Web.Services;

namespace DGCP.APPMobile.Web.Controllers
{
    public class MinistryController : ApiController
    {
        public MinistryService _ministryService;
        public MinistryController()
        {
            _ministryService = new MinistryService();
        }
        // GET api/ministry
        [ActionName("List")]
        public HttpResponseMessage GetList(int page = 1, [FromUri]List<string> selected = null, string searchCriteria = "")
        {
            var result = new HttpResponseMessage();

            try
            {
                var ministry = _ministryService.GetMinistries(page, selected, searchCriteria);

                result = Request.CreateResponse(HttpStatusCode.OK, ministry);
            }
            catch (Exception e)
            {
                result = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return result;
        }

        // GET api/ministry/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/ministry
        public void Post([FromBody]string value)
        {
        }

        // PUT api/ministry/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/ministry/5
        public void Delete(int id)
        {
        }
    }
}
