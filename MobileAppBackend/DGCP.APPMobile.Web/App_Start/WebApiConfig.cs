using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;

namespace DGCP.APPMobile.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
              name: "Route2",
                routeTemplate: "api/{controller}/{action}"
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


            /*config.Routes.MapHttpRoute(
                name: "Route2",
                routeTemplate: "api/{controller}/{action}/{page}/{procurementId}/{purchasingUnitId}",
                defaults: new { page = UrlParameter.Optional, procurementId = UrlParameter.Optional, purchasingUnitId = UrlParameter.Optional }
            );*/

            config.Routes.MapHttpRoute(
                name: "Route3",
                routeTemplate: "api/{controller}/{action}/{procurementId}/{purchasingUnitId}/{page}",
                defaults: new { procurementId = RouteParameter.Optional, purchasingUnitId = RouteParameter.Optional, page = RouteParameter.Optional }
            );

            //Recieve JSON by Default
            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);
        }
    }
}
