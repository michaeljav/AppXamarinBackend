using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using DGCP.APPMobile.Data;
using DGCP.APPMobile.Data.DTO;
using DGCP.APPMobile.Data.Enum;
using DGCP.APPMobile.Data.Helpers;
using DGCP.APPMobile.Data.UoWs;

namespace DGCP.APPMobile.Web.Services
{
    
    public class ProcurementService
    {
        private IRepository<CC_TRAMITES_COMPRAS> ProcurementRepository { get; set; }
        private IRepository<CC_UNIDAD_COMPRAS> PurchasingUnitRepository { get; set; }
        private IRepository<CL_CAPITULO> MinistryRepository { get; set; }
        private IRepository<CC_MODALIDAD_COMPRA> PurchasingModeRepository { get; set; }
        private IRepository<CC_RUBROS> SectorRepository { get; set; }
        private IRepository<CC_PUBLICACIONES> PublicationRepository { get; set; }
        private IRepository<CC_TRAMITE_ITEMS> ProcurementItemsRepository { get; set; }
        private IRepository<CC_DOM_ESTADOS> StatusRepository { get; set; }
        private IRepository<cc_publicaciones_items> PublicationItemsRepository  { get; set; }
        private AMUoW AM;

        public ProcurementService()
        {
            AM = new AMUoW(new RepositoryProvider(new RepositoryFactories()));
            ProcurementRepository = AM.Procurement;
            PurchasingUnitRepository = AM.PurchasingUnit;
            PurchasingModeRepository = AM.PurchasingMode;
            MinistryRepository = AM.Ministry;
            SectorRepository = AM.Sector;
            PublicationRepository = AM.Publication;
            ProcurementItemsRepository = AM.ProcurementItems;
            PublicationItemsRepository = AM.PublicationItems;
            StatusRepository = AM.State;
        }

        public ProcurementGeneralDescriptionDTO GetProcurementGeneralDescription(string publicationId, string period)
        {
            ProcurementGeneralDescriptionDTO procurementInfo = null;

            try
            {
                // Format Parameters
                //publicationId = !String.IsNullOrEmpty(publicationId) ? publicationId.PadLeft(6, '0') : null;
                //period = !String.IsNullOrEmpty(period) ? period.PadLeft(6, '0') : null;

                procurementInfo = (from pub in PublicationRepository.GetAll()
                                   //join proc in ProcurementRepository.GetAll() on new { ID_TRAMITE = pub.ID_TRAMITE.Replace("COD_UNIDAD_COMPRASk1=2@" + pub.COD_UNIDAD_COMPRA + "k1,2@ID_TRAMITEk1=2@", ""), COD_UNIDAD_COMPRA = pub.COD_UNIDAD_COMPRA } equals new { ID_TRAMITE = proc.ID_TRAMITE, COD_UNIDAD_COMPRA = proc.COD_UNIDAD_COMPRAS }
                                   join state in StatusRepository.GetAll() on pub.COD_ESTADO equals state.COD_ESTADO
                                   join punit in PurchasingUnitRepository.GetAll() on pub.COD_UNIDAD_COMPRA equals punit.COD_UNIDAD_COMPRAS
                                   join min in MinistryRepository.GetAll() on pub.COD_CAPITULO equals min.COD_CAPITULO
                                   join pmode in PurchasingModeRepository.GetAll() on pub.COD_MODALIDAD equals pmode.COD_MODALIDAD
                                 //  join sec in SectorRepository.GetAll() on pub.COD_RUBRO_PRINCIPAL equals sec.COD_RUBRO

                                   where (publicationId == null || pub.COD_PUBLICACION == publicationId)
                                      && (period == null || pub.COD_PERIODO_PUBLICACION == period)
                                   select new ProcurementGeneralDescriptionDTO
                                   {
                                       //ProcurementId = pub.ID_TRAMITE,
                                       PublicationId = pub.COD_PUBLICACION,
                                       Period = pub.COD_PERIODO_PUBLICACION,
                                       Ministry = new MinistryDTO
                                       {
                                           Id = min.COD_CAPITULO,
                                           Name = min.NOM_CAPITULO
                                       },
                                       PurchasingUnit = new PurchasingUnitDTO
                                       {
                                           Id = punit.COD_UNIDAD_COMPRAS,
                                           Name = punit.DES_UNIDAD
                                       },
                                       PurchasingMode = new PurchasingModeDTO
                                       {
                                           Id = pmode.COD_MODALIDAD,
                                           Name = pmode.DES_MODALIDAD
                                       },
                                       ProcessId = pub.DES_NUM_TRAMITE,
                                       ShortDescription = pub.DES_CARATULA,
                                       Description = pub.DES_TRAMITE,
                                       Sector = new SectorDTO
                                       {
                                           Id = ""/*sec.COD_RUBRO*/,
                                           Name =""/* sec.DES_RUBRO*/
                                       },
                                       PublicationDate = pub.FCH_INICIO_PUBLICACION, // Confirmar 
                                       EstimatedAdjudicationDate = pub.FCH_ESTIMADA_ADJUDICACION,
                                       StatementId = pub.COD_PLIEGO
                                   })
                               .Single();


            }
            catch (Exception e)
            {
                //Todo: Incluir Log
                Console.WriteLine("Error");
            }

            return procurementInfo;
        }

        public ProcurementContactInfoDTO GetProcurementContactInfo(string publicationId, string period)
        {
            ProcurementContactInfoDTO procurementInfo = null;

            try
            {
                // Format Parameters
                //publicationId = !String.IsNullOrEmpty(publicationId) ? publicationId.PadLeft(6, '0') : null;
               //period = !String.IsNullOrEmpty(period) ? period.PadLeft(6, '0') : null;

                procurementInfo = (from pub in PublicationRepository.GetAll()
                                   //join punit in PurchasingUnitRepository.GetAll() on pub.COD_UNIDAD_COMPRA equals punit.COD_UNIDAD_COMPRAS
                                   where (publicationId == null || pub.COD_PUBLICACION == publicationId)
                                      && (period == null || pub.COD_PERIODO_PUBLICACION == period)
                                   select new ProcurementContactInfoDTO
                                   {
                                       PublicationId = pub.COD_PUBLICACION,
                                       Period = pub.COD_PERIODO_PUBLICACION,
                                       /*PurchasingUnit = new PurchasingUnitDTO
                                       {
                                           Id = punit.COD_UNIDAD_COMPRAS,
                                           Name = punit.DES_UNIDAD
                                       },*/
                                       Contact = new ContactDTO
                                       {
                                           Name = pub.DES_CONTACTO,
                                           Email = pub.DES_EMAIL_CONTACTO
                                       }
                                   })
                               .Single();


            }
            catch (Exception e)
            {
                //Todo: Incluir Log
                Console.WriteLine("Error");
            }

            return procurementInfo;
        }

        public ProcurementReceptionInfoDTO GetProcurementReceptionInfo(string publicationId, string period)
        {
            ProcurementReceptionInfoDTO procurementInfo = null;

            try
            {
                // Format Parameters
                /*publicationId = !String.IsNullOrEmpty(publicationId) ? publicationId.PadLeft(6, '0') : null;
                period = !String.IsNullOrEmpty(period) ? period.PadLeft(6, '0') : null;*/

                procurementInfo = (from pub in PublicationRepository.GetAll()
                                   //join punit in PurchasingUnitRepository.GetAll() on pub.COD_UNIDAD_COMPRA equals punit.COD_UNIDAD_COMPRAS
                                   where (publicationId == null || pub.COD_PUBLICACION == publicationId)
                                      && (period == null || pub.COD_PERIODO_PUBLICACION == period)
                                   select new ProcurementReceptionInfoDTO
                                   {
                                       PublicationId = pub.COD_PUBLICACION,
                                       Period = pub.COD_PERIODO_PUBLICACION,
                                       /*PurchasingUnit = new PurchasingUnitDTO
                                       {
                                           Id = punit.COD_UNIDAD_COMPRAS,
                                           Name = punit.DES_UNIDAD
                                       },*/
                                       //ReceptionDate = pub.FCH_INICIO, //Confirmar
                                       StartOffersReceptionDate = pub.FCH_INICIO_RECEP_OFERTAS,
                                       EndOffersReceptionDate = pub.FCH_FIN_RECEP_OFERTAS,
                                       ExtendedOffersReceptionDate = pub.FCH_EXT_RECEP_OFERTAS,
                                       OfferAddress = pub.DES_DOMICILIO_ENTREGA_OFERTA,
                                       FirstOpeningDate = pub.FCH_PRIMERA_APERTURA,
                                       FirstOpeningHour = pub.HRA_PRIMERA_APERTURA,
                                       FirstExtendedOpeningDate = pub.FCH_EXT_PRIMERA_APERTURA,
                                       FirstExtendedOpeningHour = pub.HRA_EXT_PRIMERA_APERTURA,
                                       SecondOpeningDate = pub.FCH_SEGUNDA_APERTURA,
                                       SecondOpeningHour = pub.HRA_SEGUNDA_APERTURA,
                                       SecondExtendedOpeningDate = pub.FCH_EXT_SEGUNDA_APERTURA,
                                       SecondExtendedOpeningHour = pub.HRA_EXT_SEGUNDA_APERTURA,
                                       OpeningPlaceAddress = pub.DES_DOMICILIO_ACTO_APERTURA
                                       //ApprovalDate = pub.FCH_APROBACION
                                   })
                               .Single();


            }
            catch (Exception e)
            {
                //Todo: Incluir Log
                Console.WriteLine("Error");
            }

            return procurementInfo;
        }


        public ProcurementInfoDTO GetProcurementInfo(string procurementId, string purchasingUnitId)
        {
            ProcurementInfoDTO procurementInfo = null;

            try
            {
                // Format Parameters
                procurementId = !String.IsNullOrEmpty(procurementId) ? procurementId.PadLeft(6, '0') : null;
                purchasingUnitId = !String.IsNullOrEmpty(purchasingUnitId) ? purchasingUnitId.PadLeft(6, '0') : null;

                procurementInfo = (from proc in ProcurementRepository.GetAll()
                               join punit in PurchasingUnitRepository.GetAll() on proc.COD_UNIDAD_COMPRAS equals punit.COD_UNIDAD_COMPRAS
                               join min in MinistryRepository.GetAll() on punit.COD_CAPITULO equals min.COD_CAPITULO
                               join pmode in PurchasingModeRepository.GetAll() on proc.COD_MODALIDAD equals pmode.COD_MODALIDAD
                            //   join sec in SectorRepository.GetAll() on proc.COD_RUBRO_PRINCIPAL equals sec.COD_RUBRO
                               join pub in PublicationRepository.GetAll() on proc.COD_UNIDAD_COMPRAS equals pub.COD_UNIDAD_COMPRA into pubOP
                               from publication in pubOP.Where(p => p.ID_TRAMITE.Contains(proc.ID_TRAMITE)).DefaultIfEmpty()
                               where (procurementId == null || proc.ID_TRAMITE == procurementId)
                                  && (purchasingUnitId == null || proc.COD_UNIDAD_COMPRAS == purchasingUnitId)
                               select new ProcurementInfoDTO
                               {
                                   ProcurementId = proc.ID_TRAMITE,
                                   Period = proc.COD_PERIODO,
                                   PublicationId = publication.COD_PUBLICACION,
                                   Ministry = new MinistryDTO
                                   {
                                       Id = min.COD_CAPITULO,
                                       Name = min.NOM_CAPITULO
                                   },
                                   PurchasingUnit = new PurchasingUnitDTO
                                   {
                                       Id = punit.COD_UNIDAD_COMPRAS,
                                       Name = punit.DES_UNIDAD
                                   },
                                   PurchasingMode = new PurchasingModeDTO
                                   {
                                       Id = pmode.COD_MODALIDAD,
                                       Name = pmode.DES_MODALIDAD
                                   },
                                   ProcessId = publication.DES_NUM_TRAMITE,
                                   ShortDescription = proc.DES_CARATULA,
                                   Description = proc.DES_TRAMITE,
                                   Sector = new SectorDTO
                                   {
                                       Id = ""/*sec.COD_RUBRO*/,
                                       Name = ""/* sec.DES_RUBRO*/
                                   },
                                   PublicationDate = publication.FCH_INICIO_PUBLICACION, // Confirmar 
                                   EstimatedAdjudicationDate = proc.FCH_ESTIMADA_ADJUDICACION,
                                   StatementId = proc.NUM_PLIEGO,
                                   Contact = new ContactDTO
                                   {
                                       Name = proc.DES_CONTACTO,
                                       Email = proc.DES_EMAIL_CONTACTO
                                   },
                                   ReceptionDate = proc.FCH_INICIO, //Confirmar
                                   StartOffersReceptionDate = proc.FCH_INICIO_RECEP_OFERTAS,
                                   EndOffersReceptionDate = proc.FCH_FIN_RECEP_OFERTAS,
                                   ExtendedOffersReceptionDate = proc.FCH_EXT_RECEP_OFERTAS,
                                   OfferAddress = proc.DES_DOMICILIO_ENTREGA_OFERTA,
                                   FirstOpeningDate = proc.FCH_PRIMERA_APERTURA,
                                   FirstOpeningHour = proc.HRA_PRIMERA_APERTURA,
                                   FirstExtendedOpeningDate = proc.FCH_EXT_PRIMERA_APERTURA,
                                   FirstExtendedOpeningHour = proc.HRA_EXT_PRIMERA_APERTURA,
                                   SecondOpeningDate = proc.FCH_SEGUNDA_APERTURA,
                                   SecondOpeningHour = proc.HRA_SEGUNDA_APERTURA,
                                   SecondExtendedOpeningDate = proc.FCH_EXT_SEGUNDA_APERTURA,
                                   SecondExtendedOpeningHour = proc.HRA_EXT_SEGUNDA_APERTURA,
                                   OpeningPlaceAddress = proc.DES_DOMICILIO_ACTO_APERTURA,
                                   ApprovalDate = proc.FCH_APROBACION
                               })
                               .Single();


            } catch(Exception e)
            {
                //Todo: Incluir Log
                Console.WriteLine("Error");
            }

            return procurementInfo;
        }

        public List<ProcurementDTO> GetProcurements(int page = 1, List<string> purchasingUnitId = null, List<string> ministryId  = null,
                                                    List<string> purchaseModeId = null, string processId = "", string description = "",
                                                    List<string> sectorId = null, List<string> stateId = null, DateTime? publicationStartDate = null,
                                                    DateTime? publicationEndDate = null, DateTime? receptionStartDate = null,
                                                    DateTime? receptionEndDate = null, bool filterFlag = false, bool miPyMeFlag = false, bool configFlag = false, bool notificationFlag = false)
        {
            List<ProcurementDTO> procurement = null;
            try
            {
                // Pagination
                page = Convert.ToBoolean(page) ? page : 1;
                var skipRows = (page - 1) * 20;
                var pageSize = page * 20;

                const string procApprovedState = "03";
                const string miPyMesExceptionCode = "03";
                DateTime currentDate = DateTime.Today;
                int publicationDays = Convert.ToInt32(WebConfigurationManager.AppSettings["publicationDays"]);
                DateTime yesterdayDate = DateTime.Today.AddDays(publicationDays);

                // Format Parameters
                for (var a = 0; a <= ministryId.Count - 1; a++ )
                {
                    ministryId[a] = !String.IsNullOrEmpty(ministryId[a]) ? ministryId[a].PadLeft(4, '0') : null;
                }

                for (var b = 0; b <= stateId.Count - 1; b++)
                {
                    stateId[b] = !String.IsNullOrEmpty(stateId[b]) ? stateId[b].PadLeft(2, '0') : null;
                }

                for (var c = 0; c <= sectorId.Count - 1; c++)
                {
                    sectorId[c] = !String.IsNullOrEmpty(sectorId[c]) ? sectorId[c].PadLeft(5, '0') : null;
                }

                for (var d = 0; d <= purchasingUnitId.Count - 1; d++)
                {
                    purchasingUnitId[d] = !String.IsNullOrEmpty(purchasingUnitId[d]) ? purchasingUnitId[d].PadLeft(6, '0') : null;
                }

                if (!filterFlag)
                {
                    procurement = (from pub in PublicationRepository.GetAll()
                                   //join proc in ProcurementRepository.GetAll() on new { ID_TRAMITE = pub.ID_TRAMITE.Replace("COD_UNIDAD_COMPRASk1=2@" + pub.COD_UNIDAD_COMPRA + "k1,2@ID_TRAMITEk1=2@", ""), COD_UNIDAD_COMPRA = pub.COD_UNIDAD_COMPRA } equals new { ID_TRAMITE = proc.ID_TRAMITE, COD_UNIDAD_COMPRA = proc.COD_UNIDAD_COMPRAS }
                                   join state in StatusRepository.GetAll() on pub.COD_ESTADO equals state.COD_ESTADO
                                   join punit in PurchasingUnitRepository.GetAll() on pub.COD_UNIDAD_COMPRA equals punit.COD_UNIDAD_COMPRAS
                                  // join sector in SectorRepository.GetAll() on pub.COD_RUBRO_PRINCIPAL equals sector.COD_RUBRO
                                   where (/*(sectorId.Count == 0 || sectorId.Contains(pub.COD_RUBRO_PRINCIPAL)) 
                                          && */(pub.COD_ESTADO == procApprovedState)
                                          && ((pub.FCH_EXT_RECEP_OFERTAS != null && pub.FCH_EXT_RECEP_OFERTAS >= currentDate) || (pub.FCH_FIN_RECEP_OFERTAS >= currentDate))
                                          && (notificationFlag == false || (notificationFlag && pub.FCH_INICIO_PUBLICACION == yesterdayDate)))
                                   select new ProcurementDTO
                                   {
                                       PublicationId = pub.COD_PUBLICACION,
                                       Period = pub.COD_PERIODO_PUBLICACION,
                                       PurchasingUnit = new PurchasingUnitDTO
                                       {
                                           Id = punit.COD_UNIDAD_COMPRAS,
                                           Name = punit.DES_UNIDAD,
                                           Telephone = punit.des_telefono
                                       },
                                       ProcessId = pub.DES_NUM_TRAMITE,
                                       Description = pub.DES_TRAMITE,
                                       ReceptionEndDate = (pub.FCH_EXT_RECEP_OFERTAS ?? pub.FCH_FIN_RECEP_OFERTAS),
                                       Sector = new SectorDTO
                                       {
                                           Id = ""/*sector.COD_RUBRO*/,
                                           Name = ""/*sector.DES_RUBRO*/
                                       },
                                       Contact = new ContactDTO
                                       {
                                           Name = pub.DES_CONTACTO,
                                           Email = pub.DES_EMAIL_CONTACTO

                                       },
                                       //ApprovalDate = proc.FCH_APROBACION,
                                       Link = ((pub.URL_DETALLE == null || pub.URL_DETALLE.Trim() == string.Empty) ?
                                            "http://acceso.comprasdominicana.gov.do/compras/publicaciones/consultas/procesosdecompras/edicion.jsp?seleccion=COD_PERIODO_PUBLICACIONk1:2@+" 
                                            + pub.COD_PERIODO_PUBLICACION + "+k1;2@COD_PUBLICACIONk1:2@+" + pub.COD_PUBLICACION + "&OP_RETORNO=OperacionBuscar&operacion=visualizarVO"
                                            : pub.URL_DETALLE),
                                       Status = state.DES_ESTADO
                                   })
                       .OrderBy(proc => proc.ReceptionEndDate)
                       .Take(pageSize)
                       .Skip(skipRows)
                       .ToList();



                 

                }
                else
                {

                    procurement = (from pub in PublicationRepository.GetAll()
                                   //join proc in ProcurementRepository.GetAll() on new { ID_TRAMITE = pub.ID_TRAMITE.Replace("COD_UNIDAD_COMPRASk1=2@" + pub.COD_UNIDAD_COMPRA + "k1,2@ID_TRAMITEk1=2@", ""), COD_UNIDAD_COMPRA = pub.COD_UNIDAD_COMPRA } equals new { ID_TRAMITE = proc.ID_TRAMITE, COD_UNIDAD_COMPRA = proc.COD_UNIDAD_COMPRAS }
                                   join state in StatusRepository.GetAll() on pub.COD_ESTADO equals state.COD_ESTADO
                                   join punit in PurchasingUnitRepository.GetAll() on pub.COD_UNIDAD_COMPRA equals punit.COD_UNIDAD_COMPRAS
                                 //  join sector in SectorRepository.GetAll() on pub.COD_RUBRO_PRINCIPAL equals sector.COD_RUBRO
                                   where
                                       (purchasingUnitId.Count == 0 ||
                                        purchasingUnitId.Contains(pub.COD_UNIDAD_COMPRA))
                                       && (ministryId.Count == 0 || ministryId.Contains(pub.COD_CAPITULO))
                                       && (purchaseModeId.Count == 0 || purchaseModeId.Contains(pub.COD_MODALIDAD))
                                       && (sectorId.Count == 0 || sectorId.Contains(pub.COD_RUBRO_PRINCIPAL))
                                       && (configFlag == false && stateId.Count == 0 || (configFlag && pub.COD_ESTADO == procApprovedState) || (configFlag == false && stateId.Contains(pub.COD_ESTADO)))
                                       &&
                                       (receptionStartDate == null ||
                                        /*(proc.FCH_INICIO_RECEP_OFERTAS >= receptionStartDate &&
                                         proc.FCH_FIN_RECEP_OFERTAS >= receptionStartDate)*/
                                        (pub.FCH_INICIO_RECEP_OFERTAS >= receptionStartDate))
                                       &&
                                       (receptionEndDate == null || 
                                        /*(proc.FCH_FIN_RECEP_OFERTAS <= receptionEndDate &&
                                         proc.FCH_INICIO_RECEP_OFERTAS <= receptionEndDate)*/
                                        pub.FCH_FIN_RECEP_OFERTAS <= receptionEndDate)
                                       &&
                                       ((notificationFlag && pub.FCH_INICIO_PUBLICACION == yesterdayDate) || (notificationFlag == false && publicationStartDate == null) || (notificationFlag == false && pub.FCH_INICIO_PUBLICACION >= publicationStartDate))
                                       && (notificationFlag || (notificationFlag == false && publicationEndDate == null) || (pub.FCH_INICIO_PUBLICACION <= publicationEndDate && notificationFlag == false))
                                       &&
                                       (String.IsNullOrEmpty(processId) ||
                                        pub.DES_NUM_TRAMITE.Contains(processId))
                                       && (String.IsNullOrEmpty(description) || pub.DES_TRAMITE.Contains(description))
                                       && (miPyMeFlag == false || pub.COD_TIPO_EXCEPCION == miPyMesExceptionCode)
                                       && (notificationFlag || ((pub.FCH_EXT_RECEP_OFERTAS != null && pub.FCH_EXT_RECEP_OFERTAS >= currentDate) || (pub.FCH_FIN_RECEP_OFERTAS >= currentDate)))
                                   select new ProcurementDTO
                                   {
                                       PublicationId = pub.COD_PUBLICACION,
                                       Period = pub.COD_PERIODO_PUBLICACION,
                                       PurchasingUnit = new PurchasingUnitDTO
                                       {
                                           Id = punit.COD_UNIDAD_COMPRAS,
                                           Name = punit.DES_UNIDAD,
                                           Telephone = punit.des_telefono
                                       },
                                       ProcessId = pub.DES_NUM_TRAMITE,
                                       Description = pub.DES_TRAMITE,
                                       ReceptionEndDate = (pub.FCH_EXT_RECEP_OFERTAS ?? pub.FCH_FIN_RECEP_OFERTAS),
                                       Sector = new SectorDTO
                                       {
                                           Id = ""/*sector.COD_RUBRO*/,
                                           Name = ""/*sector.DES_RUBRO*/
                                       },
                                       Contact = new ContactDTO
                                       {
                                            Name = pub.DES_CONTACTO,
                                            Email = pub.DES_EMAIL_CONTACTO
                                                         
                                       },
                                       //ApprovalDate = proc.FCH_APROBACION,
                                       Link = ((pub.URL_DETALLE == null || pub.URL_DETALLE.Trim() == string.Empty) ?
                                            "http://acceso.comprasdominicana.gov.do/compras/publicaciones/consultas/procesosdecompras/edicion.jsp?seleccion=COD_PERIODO_PUBLICACIONk1:2@+" 
                                            + pub.COD_PERIODO_PUBLICACION + "+k1;2@COD_PUBLICACIONk1:2@+" + pub.COD_PUBLICACION + "&OP_RETORNO=OperacionBuscar&operacion=visualizarVO"
                                            : pub.URL_DETALLE),
                                       Status = state.DES_ESTADO
                                   })
                        .OrderBy(proc => proc.ReceptionEndDate)
                        .Take(pageSize)
                        .Skip(skipRows)
                        .ToList();

                   
                }


            }   catch(Exception e)
            {
                //Todo: Incluir Log
                Console.WriteLine("Error", e.Message);
            }

            return procurement;

        }

        public List<ProcurementItemDTO> GetProcurementItems(string publicationId, string period, int page = 1)
        {
            List<ProcurementItemDTO> procurementItems = null;
            try
            {

                // Pagination
                page = Convert.ToBoolean(page) ? page : 1;
                var skipRows = (page - 1) * 20;
                var pageSize = page * 20;

                // Format Parameters
                //publicationId = !String.IsNullOrEmpty(publicationId) ? publicationId.PadLeft(6, '0') : null;
                //period = !String.IsNullOrEmpty(period) ? period.PadLeft(6, '0') : null;

                procurementItems =     (from procItem in PublicationItemsRepository.GetAll() 
                                        /*join punit in PurchasingUnitRepository.GetAll() on procItem.COD_UNIDAD_COMPRAS equals
                                          punit.COD_UNIDAD_COMPRAS*/
                                       
                                         join  punit in SectorRepository.GetAll() 
                                        on procItem.COD_UNSPSC
                                            equals punit.COD_RUBRO   into gj
                                         from subpet in gj.DefaultIfEmpty()

                                        where procItem.COD_PUBLICACION == publicationId && procItem.COD_PERIODO_PUBLICACION == period
                                       select new ProcurementItemDTO
                                                  {
                                                      PublicationId = procItem.COD_PUBLICACION,
                                                      Period = procItem.COD_PERIODO_PUBLICACION,
                                                      /*PurchasingUnit = new PurchasingUnitDTO
                                                      {
                                                          Id = punit.COD_UNIDAD_COMPRAS,
                                                          Name = punit.DES_UNIDAD
                                                      },*/
                                                      ItemId = procItem.COD_PUBLICACION_ITEM,
                                                      Description = procItem.DES_ITEM_TRAMITE,
                                                      Quantity = procItem.VLR_CANTIDAD,
                                                      DesUNSPSC= (subpet.COD_RUBRO != null ? subpet.COD_RUBRO + "-" : string.Empty)+ (subpet.DES_RUBRO != null ? subpet.DES_RUBRO : string.Empty)
                                       }
                                   )
                                   .OrderByDescending(proc => proc.Description)
                                   .Take(pageSize)
                                   .Skip(skipRows)
                                   .ToList();


            } catch(Exception e)
            {
                Console.WriteLine("Error");
            }

            return procurementItems;

        }

        public ProcurementTinyUrlDTO GetProcurementTinyURL(string publicationId, string period)
        {
            ProcurementTinyUrlDTO procurementTinyURL = null;
            try
            {
                ProcurementDTO procurement = (from pub in PublicationRepository.GetAll()
                                              where pub.COD_PUBLICACION == publicationId && pub.COD_PERIODO_PUBLICACION == period
                                              select new ProcurementDTO
                                                         {
                                                             PublicationId = pub.COD_PUBLICACION,
                                                             Period = pub.COD_PERIODO_PUBLICACION,
                                                             Link = ((pub.URL_DETALLE == null || pub.URL_DETALLE.Trim() == string.Empty) ?
                                                                "http://acceso.comprasdominicana.gov.do/compras/publicaciones/consultas/procesosdecompras/edicion.jsp?seleccion=COD_PERIODO_PUBLICACIONk1:2@+" 
                                                                + pub.COD_PERIODO_PUBLICACION + "+k1;2@COD_PUBLICACIONk1:2@+" + pub.COD_PUBLICACION + "&OP_RETORNO=OperacionBuscar&operacion=visualizarVO"
                                                                : pub.URL_DETALLE)
                                                         }).Single();

                if (procurement != null)
                {

                    var uShortenerService = new UrlShortenerService();
                    var tinyURL = "";
                    var result = uShortenerService.ShortUrl((int)UrlShortenerServices.Google, procurement.Link, ref tinyURL);
                    procurementTinyURL = new ProcurementTinyUrlDTO();
                    procurementTinyURL.PublicationId = procurement.PublicationId;
                    procurementTinyURL.Period = procurement.Period;

                    //tinyURl or DefaultLink
                    procurementTinyURL.TinyURL = result ? tinyURL : procurement.Link;
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error");
            }

            return procurementTinyURL;

        }

    }
}
