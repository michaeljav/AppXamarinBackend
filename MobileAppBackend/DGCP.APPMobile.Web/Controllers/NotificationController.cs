using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using DGCP.APPMobile.Web.Services;
using DGCP.APPMobile.Data.Enum;

namespace DGCP.APPMobile.Web.Controllers
{
    public class NotificationController : ApiController
    {
        public NotificationService _notificationService;
        public GCMService _GcmService;
        public NotificationController()
        {
            _notificationService = new NotificationService();
            _GcmService = new GCMService();
        }

        // GET api/ministry
        [ActionName("AddDevice")]
        public HttpResponseMessage GetAddDevice(string deviceToken, [FromUri]List<string> purchasingUnitId = null, [FromUri]List<string> ministryId = null,
                                                    [FromUri]List<string> purchaseModeId = null, [FromUri]List<string> sectorId = null, [FromUri]bool miPyMeFlag = false, [FromUri] int operatingSystemId = 1)
        {
            var result = new HttpResponseMessage();
            
            try
            {
                var notification = _notificationService.AddDeviceConfiguration(deviceToken, purchasingUnitId, ministryId, purchaseModeId, sectorId, miPyMeFlag, operatingSystemId);

                result = Request.CreateResponse(HttpStatusCode.OK, notification);
            }
            catch (Exception e)
            {
                result = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return result;
        }

        // GET api/ministry
        [ActionName("DisableDevice")]
        public HttpResponseMessage GetDisableDevice(string deviceToken, int operatingSystemId = 1)
        {
            var result = new HttpResponseMessage();

            try
            {
                var notification = _notificationService.DisableDeviceConfiguration(deviceToken, operatingSystemId);

                result = Request.CreateResponse(HttpStatusCode.OK, notification);
            }
            catch (Exception e)
            {
                result = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return result;
        }

        // GET api/ministry
        [ActionName("SendNotifications")]
        public HttpResponseMessage GetSendNotifications(string username, string password)
        {
            /*using (System.IO.StreamWriter file = new System.IO.StreamWriter("~/App_Data"))
            {
                file.WriteLine("Entre");
                file.Flush();
            }*/
            const string localhost = "::1";
            var remoteIPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            const string usernameval = "Compr@sRD";
            const string passwordval = "Fr1k1t@k1$2014";

            var result = new HttpResponseMessage();

            try
            {
                if (remoteIPAddress == localhost && username == usernameval && password == passwordval)
                {
                    var notification = _GcmService.SendNotifications();

                    result = Request.CreateResponse(HttpStatusCode.OK, notification);
                } else
                {
                    result = Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception e)
            {
                result = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return result;
        }


        // GET api/notification/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/notification
        public void Post([FromBody]string value)
        {
        }

        // PUT api/notification/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/notification/5
        public void Delete(int id)
        {
        }
    }
}
