using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DGCP.APPMobile.Data;
using DGCP.APPMobile.Data.Enum;
using DGCP.APPMobile.Data.Helpers;
using DGCP.APPMobile.Data.UoWs;

namespace DGCP.APPMobile.Web.Services
{
    public class NotificationService
    {
        private IRepository<Dispositivo> DeviceRepository { get; set; }
        private IRepository<ConfiguracionNotificacion> NotConfigRepository { get; set; }
        private TransactionLogService tranLogService { get; set; }
        private AMUoW AM;

        public NotificationService()
        {
            AM = new AMUoW(new RepositoryProvider(new RepositoryFactories()));
            DeviceRepository = AM.Device;
            NotConfigRepository = AM.NotificationConfiguration;
            //Set the TransactionLogService
            tranLogService = new TransactionLogService(AM);
            
        }


        private void DisableNotificationConfig(int catalogTypeId, int deviceId, DateTime currentTime)
        {
            try
            {
                var notConfigUpdateList = NotConfigRepository.GetAll().Where(n => n.TipoCatalogoId == catalogTypeId && n.DispositivoId == deviceId).ToList();

                foreach(var nConfigObj in notConfigUpdateList)
                {
                    nConfigObj.EstadoId = (int)ConfigState.Inactive;
                    nConfigObj.FechaModificacion = currentTime;
                    NotConfigRepository.Update(nConfigObj);
                }
            } catch(Exception e)
            {
                //TODO: Log Message
            }

        }

        private void AddNotificationConfig(int deviceId, int catalogTypeId, string catalogId, int stateId, DateTime registrationDate)
        {
            try
            {
                var notConfig = new ConfiguracionNotificacion
                                    {
                                        DispositivoId = deviceId,
                                        TipoCatalogoId = catalogTypeId,
                                        CatalogoId = catalogId,
                                        EstadoId = stateId,
                                        FechaRegistro = registrationDate
                                    };
                NotConfigRepository.Add(notConfig);
                
            } catch(Exception e)
            {
                
            }
        }

        private void AddNotificationConfig(List<string> catalogList, int deviceId, int catalogTypeId, DateTime currentTime)
        {
            try
            {
                DisableNotificationConfig(catalogTypeId, deviceId, currentTime);

                if (catalogList != null && catalogList.Count > 0)
                {
                    foreach (var catalogId in catalogList)
                    {
                        AddNotificationConfig(deviceId, catalogTypeId, catalogId, (int)ConfigState.Active, currentTime);
                    }
                }

                
            } catch(Exception e)
            {
                //TODO: Log Message
            }
        }

        public bool DisableDeviceConfiguration(string deviceToken, int operatingSystemId = 1)
        {
            try
            {
                DateTime currentTime = DateTime.Now;
                var device = DeviceRepository.GetAll().SingleOrDefault(d => d.GCMRegistroId == deviceToken && d.SistemaOperativoId == operatingSystemId);
                if (device != null)
                {
                    device.FechaModificacion = currentTime;
                    device.EstadoId = (int) ConfigState.Inactive;
                    DeviceRepository.Update(device);

                    //Log the Transaction
                    const string transactionMessage = "Dispositivo Deshabilitado";
                    tranLogService.AddTransactionLog(device, (int)TransactionType.DeviceDisabled, currentTime, transactionMessage);
                    AM.Commit();
                }
                
            } catch(Exception e)
            {
                //Todo: Log Message
                return false;
            }
            return true;
        }

        public bool AddDeviceConfiguration(string deviceToken, List<string> purchasingUnitId = null, List<string> ministryId = null,
                                                    List<string> purchaseModeId = null, List<string> sectorId = null, bool miPyMeFlag = false, int operatingSystemId = 1)
        {
            var currentTime = DateTime.Now;
            string transactionMessage = "";
            try
            {
                var device = DeviceRepository.GetAll().SingleOrDefault(d => d.GCMRegistroId == deviceToken && d.SistemaOperativoId == operatingSystemId);
                Dispositivo deviceEntity = null;
                if (device == null)
                {
                    deviceEntity = new Dispositivo();
                    deviceEntity.GCMRegistroId = deviceToken;
                    deviceEntity.EstadoId = (int) ConfigState.Active;
                    deviceEntity.FechaRegistro = currentTime;
                    deviceEntity.SistemaOperativoId = operatingSystemId;
                    DeviceRepository.Add(deviceEntity);

                    //Log the Transaction - Device Registered
                    transactionMessage = "Dispositivo registrado";
                    tranLogService.AddTransactionLog(deviceEntity, (int)TransactionType.Registration, currentTime, transactionMessage);                   

                } else
                {
                    device.EstadoId = (int)ConfigState.Active;
                    device.FechaModificacion = currentTime;
                    DeviceRepository.Update(device);

                    deviceEntity = device;
                }

                var deviceId = deviceEntity.Id;
                
                //Add Notification Configuration for PurchasingUnit
                const int pUnitCatalogTypeId = (int)CatalogType.PurchasingUnit;
                AddNotificationConfig(purchasingUnitId, deviceId, pUnitCatalogTypeId, currentTime);

                //Add Notification Configuration for Ministry
                const int ministryCatalogTypeId = (int)CatalogType.Ministry;
                AddNotificationConfig(ministryId, deviceId, ministryCatalogTypeId, currentTime);

                //Add Notification Configuration for purchaseMode
                const int pModeCatalogTypeId = (int)CatalogType.PurchasingMode;
                AddNotificationConfig(purchaseModeId, deviceId, pModeCatalogTypeId, currentTime);

                //Add Notification Configuration for sector
                const int sectorCatalogTypeId = (int)CatalogType.Sector;
                AddNotificationConfig(sectorId, deviceId, sectorCatalogTypeId, currentTime);

                //Add Notification for MiPyMe
                const int miPyMeCatalogId = (int)CatalogType.MiPyMe;
                DisableNotificationConfig(miPyMeCatalogId, deviceId, currentTime);
                if (miPyMeFlag)
                {
                    AddNotificationConfig(deviceId, miPyMeCatalogId, Convert.ToString((Convert.ToInt32(miPyMeFlag))), (int)ConfigState.Active, currentTime);
                }

                //Log the Transaction - Device Configuration
                transactionMessage = "Configuración de Dispositivo Actualizada";
                tranLogService.AddTransactionLog(deviceEntity, (int)TransactionType.ConfigurationUpdated, currentTime, transactionMessage); 
                AM.Commit();


            } catch(Exception e)
            {
                //Todo: Log the transaction
                return false;
            }

            return true;

        }

    }
}
