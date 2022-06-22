using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Script.Serialization;
using DGCP.APPMobile.Data;
using DGCP.APPMobile.Data.DTO;
using DGCP.APPMobile.Data.Enum;
using DGCP.APPMobile.Data.Helpers;
using DGCP.APPMobile.Data.UoWs;
using PushSharp;
using PushSharp.Apple;
using PushSharp.Core;
using OperatingSystem = DGCP.APPMobile.Data.Enum.OperatingSystem;

namespace DGCP.APPMobile.Web.Services
{
    public class GCMService
    {
        private IRepository<CC_PUBLICACIONES> PublicationRepository { get; set; }
        private IRepository<Dispositivo> DeviceRepository { get; set; }
        private AMUoW AM;
        private const string GCMApiURL = "https://android.googleapis.com/gcm/send";
        private const string AuthKey = "-3MZ2arE9Dh20";
        private const int regIdsPerNotification = 1000;
        private TransactionLogService tranLogService { get; set; }
        const int activeState = (int)ConfigState.Active;
        
        public GCMService()
        {
            AM = new AMUoW(new RepositoryProvider(new RepositoryFactories()));
            DeviceRepository = AM.Device;
            PublicationRepository = AM.Publication;
            tranLogService = new TransactionLogService(AM);
        }

        public String SendNotifications()
        {
            var result = string.Empty;

            try
            {
                const string procApprovedState = "03";
                const string miPyMesExceptionCode = "03";
                int publicationDays = Convert.ToInt32(WebConfigurationManager.AppSettings["publicationDays"]);
                DateTime yesterdayDate = DateTime.Today.AddDays(publicationDays);
				DateTime currentDate = DateTime.Today;
                var devicesNotConfigured = new List<Dispositivo>();
                var appleDevicesNotConfigured = new List<Dispositivo>();
                var devicesIdForGeneralNotification = new List<Dispositivo>();
                var devicesIdForAppleGeneralNotification = new List<Dispositivo>();

                var procCountforDevsNotConfigured = (from pub in PublicationRepository.GetAll()
                                                     where ((pub.FCH_INICIO_PUBLICACION == yesterdayDate)
                                                     && (pub.COD_ESTADO == procApprovedState)
                                                     && ((pub.FCH_EXT_RECEP_OFERTAS != null && pub.FCH_EXT_RECEP_OFERTAS >= currentDate) || (pub.FCH_FIN_RECEP_OFERTAS >= currentDate)))
                                                     select pub).Count();
													 
                if (procCountforDevsNotConfigured > 0)
                {

                    //Devices with notifications actived.
                    var devicesActivatedList = DeviceRepository.GetAll().Where(d => d.EstadoId == activeState).ToList();

                    foreach (var device in devicesActivatedList)
                    {
                        if (device.ConfiguracionNotificacion.Where(c => c.EstadoId == activeState).Any())
                        {
                            var devNotConfig = device.ConfiguracionNotificacion;
                            var ministryCatalogTypeId = (int) CatalogType.Ministry;
                            var pUnitCatalogTypeId = (int) CatalogType.PurchasingUnit;
                            var pModeCatalogTypeId = (int) CatalogType.PurchasingMode;
                            var sectorCatalogTypeId = (int) CatalogType.Sector;
                            var miPyMeCatalogTypeId = (int) CatalogType.MiPyMe;

                            var ministryList =
                                devNotConfig.Where(
                                    c => c.TipoCatalogoId == ministryCatalogTypeId && c.EstadoId == activeState).Select(
                                        c => c.CatalogoId).ToList();
                            var purchasingUnitList =
                                devNotConfig.Where(
                                    c => c.TipoCatalogoId == pUnitCatalogTypeId && c.EstadoId == activeState).Select(
                                        c => c.CatalogoId).ToList();
                            var pModeList =
                                devNotConfig.Where(
                                    c => c.TipoCatalogoId == pModeCatalogTypeId && c.EstadoId == activeState).Select(
                                        c => c.CatalogoId).ToList();
                            var sCatalogList =
                                devNotConfig.Where(
                                    c => c.TipoCatalogoId == sectorCatalogTypeId && c.EstadoId == activeState).Select(
                                        c => c.CatalogoId).ToList();
                            var mCatalogList =
                                devNotConfig.Where(
                                    c => c.TipoCatalogoId == miPyMeCatalogTypeId && c.EstadoId == activeState).Select(
                                        c => c.CatalogoId).ToList();

                            var procurementsCount = (from pub in PublicationRepository.GetAll()
                                                     where (
                                                               (ministryList.Count == 0 ||
                                                                ministryList.Contains(pub.COD_CAPITULO))
                                                               &&
                                                               (purchasingUnitList.Count == 0 ||
                                                                purchasingUnitList.Contains(pub.COD_UNIDAD_COMPRA))
                                                               &&
                                                               (pModeList.Count == 0 ||
                                                                pModeList.Contains(pub.COD_MODALIDAD))
                                                               &&
                                                               (sCatalogList.Count == 0 ||
                                                                sCatalogList.Contains(pub.COD_RUBRO_PRINCIPAL))
                                                               &&
                                                               (mCatalogList.Count == 0 ||
                                                                pub.COD_TIPO_EXCEPCION == miPyMesExceptionCode)
                                                               && (pub.FCH_INICIO_PUBLICACION == yesterdayDate)
                                                               && (pub.COD_ESTADO == procApprovedState)
															   && ((pub.FCH_EXT_RECEP_OFERTAS != null && pub.FCH_EXT_RECEP_OFERTAS >= currentDate) || (pub.FCH_FIN_RECEP_OFERTAS >= currentDate))
                                                           )
                                                     select pub).Count();

                            //result = Convert.ToString(procurementsCount);

                            if (procurementsCount > 0)
                            {
                                var devicesForNotification = new List<Dispositivo>();
                                devicesForNotification.Add(device);
                                string notificationMessage = procurementsCount +
                                                             " procesos de compras nuevos de acuerdo a sus preferencias.";
                                if (device.SistemaOperativoId == (int) OperatingSystem.Android)
                                {
                                    SendSingleNotification(devicesForNotification, "ComprasRD", notificationMessage);
                                } else if (device.SistemaOperativoId == (int) OperatingSystem.IOS)
                                {
                                    SendSingleAppleNotification(devicesForNotification, notificationMessage);
                                }

                            }

                        }
                        else
                        {
                            if (device.SistemaOperativoId == (int)OperatingSystem.Android)
                            {
                                devicesNotConfigured.Add(device);
                            }
                            else if(device.SistemaOperativoId == (int) OperatingSystem.IOS)
                            {
                                appleDevicesNotConfigured.Add(device);
                            }
                        }

                    }

                    if (devicesNotConfigured.Count > 0)
                    {

                        /*if (procCountforDevsNotConfigured > 0)
                        {*/
                            string notificationMessage = procCountforDevsNotConfigured +
                                                         " procesos de compras nuevos de acuerdo a sus preferencias.";

                            // For Android Devices
                            foreach (var device in devicesNotConfigured)
                            {
                                devicesIdForGeneralNotification.Add(device);
                                if (devicesIdForGeneralNotification.Count == regIdsPerNotification)
                                {
                                    SendSingleNotification(devicesIdForGeneralNotification, "ComprasRD",
                                                           notificationMessage);
                                    devicesIdForGeneralNotification.Clear();
                                }

                            }

                            //the Last Notifications for Android Devices
                            if (devicesIdForGeneralNotification.Count > 0)
                            {
                                SendSingleNotification(devicesIdForGeneralNotification, "ComprasRD", notificationMessage);
                                devicesIdForGeneralNotification.Clear();
                            }

                        //}
                    }

                    if (appleDevicesNotConfigured.Count > 0)
                    {
                        /*if (procCountforDevsNotConfigured > 0)
                        {*/

                        string notificationMessage = procCountforDevsNotConfigured +
                                                     " procesos de compras nuevos de acuerdo a sus preferencias.";
                        //For Apple Devices
                        foreach (var device in appleDevicesNotConfigured)
                        {
                            devicesIdForGeneralNotification.Add(device);
                            if (devicesIdForGeneralNotification.Count == regIdsPerNotification)
                            {
                                SendSingleAppleNotification(devicesIdForGeneralNotification, notificationMessage);
                                devicesIdForGeneralNotification.Clear();
                            }

                        }

                        //the Last Notifications for Apple Devices
                        if (devicesIdForGeneralNotification.Count > 0)
                        {
                            SendSingleAppleNotification(devicesIdForGeneralNotification, notificationMessage);
                            devicesIdForGeneralNotification.Clear();
                        }

                    //}

                    }
                  }
                    



                

            } catch(Exception e)
            {                
                return result;
            }

            return result;

        }

        public string SendSingleNotification(List<Dispositivo> devicesList, string titleVal, string messageVal)
        {
            var registrationIdsList = new List<string>();
            var GCMResponseString = String.Empty;
            var currentDate = DateTime.Today;
            const int timeToLive = 28800;
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(GCMApiURL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Headers.Add("Authorization", "key=" + AuthKey);
                httpWebRequest.Method = "POST";

                //List all the DevicesIds
                registrationIdsList.AddRange(devicesList.Select(device => device.GCMRegistroId));

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = new JavaScriptSerializer().Serialize(new
                    {
                        data = new { title = titleVal, message = messageVal },
                        registration_ids = registrationIdsList, time_to_live = timeToLive // 8 Hours in the GCM Storage if the Device is offline
                    });

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        GCMResponseString = streamReader.ReadToEnd();
                        var GCMResponse = new JavaScriptSerializer().Deserialize<GCMResponseDTO>(GCMResponseString);

                        for (var k = 0; k <= GCMResponse.results.Count - 1; k++ )
                        {
                            var result = GCMResponse.results[k];
                            var device = devicesList[k];
                            if (result.error != null)
                            {
                                //Log the Transaction if occurs an error
                                const int TransactionTypeId = (int) TransactionType.NotificationNotSended;
                                tranLogService.AddTransactionLog(device, TransactionTypeId, currentDate, "Error en Respuesta de GCM", result.error);

                                //Todo: if error is NotRegistered, disable the device for recieve notification. Source: https://developer.android.com/google/gcm/http.html
                            }
                            else
                            {
                                //If the notification was sended
                                const int TransactionTypeId = (int)TransactionType.NotificationSended;
                                tranLogService.AddTransactionLog(device, TransactionTypeId, currentDate, "Notificación Enviada", result.message_id);

                                //Todo: if registration_id is not null in result, then change the GCMRegistrationId of device. Source: https://developer.android.com/google/gcm/http.html
                            }
                            AM.Commit();
                            
                        }


                    }
                }


            }catch(Exception e)
            {
                //Todo: log if notification can not be sended.
                return GCMResponseString;
            }

            return GCMResponseString;
        }

        public string SendSingleAppleNotification(List<Dispositivo> devicesList, string messageVal)
        {
            var responseString = String.Empty;
            try
            {
                foreach(var device in devicesList)
                {
                    
                    var push = new PushBroker();
                    push.OnNotificationSent += NotificationSent;
                    push.OnChannelException += ChannelException;
                    push.OnServiceException += ServiceException;
                    push.OnNotificationFailed += NotificationFailed;
                    push.OnDeviceSubscriptionExpired += DeviceSubscriptionExpired;
                    push.OnChannelCreated += ChannelCreated;
                    push.OnChannelDestroyed += ChannelDestroyed;

                    string path = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
                    var appleCert = File.ReadAllBytes("c:\\Apple_Development_Push_Services_Certificate.p12");
                   //Registering the Apple Service and sending an iOS Notification
                    push.RegisterAppleService(new ApplePushChannelSettings(appleCert, "123456"));
                    push.QueueNotification(new AppleNotification()
                                               .ForDeviceToken(device.GCMRegistroId)
                                               .WithAlert(messageVal).WithExpiry(DateTime.Now.AddHours(8)));
                }

            } catch(Exception e)
            {
                return responseString;
            }
            return responseString;
        }

        //Push Sharp Events (Only using with Apple Notifications)

        void NotificationSent(object sender, INotification notification)
        {   
            try
            {

                //Console.WriteLine("Sent: " + sender + " -> " + ((AppleNotification) notification).DeviceToken);
                var currentDate = DateTime.Now;
                var deviceToken = ((AppleNotification)notification).DeviceToken;
                var device = DeviceRepository.GetAll().Single(d => d.GCMRegistroId == deviceToken);

                const int TransactionTypeId = (int)TransactionType.NotificationSended;
                tranLogService.AddTransactionLog(device, TransactionTypeId, currentDate, "Notificación Enviada", "Sent");
                AM.Commit();
            }
            catch (Exception e)
            {

            }
         }

        void NotificationFailed(object sender, INotification notification, Exception notificationFailureException)
        {
            try
            {
                //Console.WriteLine("Failure: " + sender + " -> " + notificationFailureException.Message + " -> " + notification);
                var currentDate = DateTime.Now;
                var deviceToken = ((AppleNotification) notification).DeviceToken;
                var device = DeviceRepository.GetAll().Single(d => d.GCMRegistroId == deviceToken);

                //Log the Transaction if occurs an error
                const int TransactionTypeId = (int)TransactionType.NotificationNotSended;
                tranLogService.AddTransactionLog(device, TransactionTypeId, currentDate, "Error en Respuesta de APNS", notificationFailureException.Message);
                AM.Commit();

            }
            catch (Exception e)
            {

            }
        }

        void ChannelException(object sender, IPushChannel channel, Exception exception)
        {
            Console.WriteLine("Channel Exception: " + sender + " -> " + exception);
        }

        void ServiceException(object sender, Exception exception)
        {
            Console.WriteLine("Service Exception: " + sender + " -> " + exception);
        }

        void DeviceSubscriptionExpired(object sender, string expiredDeviceSubscriptionId, DateTime timestamp, INotification notification)
        {
            Console.WriteLine("Device Subscription Expired: " + sender + " -> " + expiredDeviceSubscriptionId);
        }

        void ChannelDestroyed(object sender)
        {
            Console.WriteLine("Channel Destroyed for: " + sender);
        }

        void ChannelCreated(object sender, IPushChannel pushChannel)
        {
            Console.WriteLine("Channel Created for: " + sender);
        }



        
    }
}
