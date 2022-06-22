using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DGCP.APPMobile.Data;
using DGCP.APPMobile.Data.DTO;
using DGCP.APPMobile.Data.Helpers;
using DGCP.APPMobile.Data.UoWs;

namespace DGCP.APPMobile.Web.Services
{
    
    public class PurchasingModeService
    {
        private IRepository<CC_MODALIDAD_COMPRA> PurchasingModeRepository;
        private AMUoW AM;

        public PurchasingModeService()
        {
            AM = new AMUoW(new RepositoryProvider(new RepositoryFactories()));
            PurchasingModeRepository = AM.PurchasingMode;
        }

        public List<PurchasingModeDTO> GetPurchasingModes(int page = 1, List<string> selected = null, string searchCriteria = "")
        {
            List<PurchasingModeDTO> PurchasingModeList = null;
            List<PurchasingModeDTO> PurchasingModeSelectedList = null;
            List<PurchasingModeDTO> PurchasingModeLists = new List<PurchasingModeDTO>();

            try
            {
                // Pagination
                page = Convert.ToBoolean(page) ? page : 1;
                var skipRows = (page - 1) * 40;
                var pageSize = page * 40;

                if (selected.Count > 0)
                {
                    PurchasingModeSelectedList = PurchasingModeRepository.GetAll()
                                    .Where(pm => selected.Contains(pm.COD_MODALIDAD))
                                    .Select(pm => new PurchasingModeDTO
                                    {
                                        Id = pm.COD_MODALIDAD,
                                        Name = pm.DES_MODALIDAD
                                    })
                                    .OrderBy(pm => pm.Name)
                                    .ToList();
                }

                PurchasingModeList = PurchasingModeRepository.GetAll()
                                     .Where(pm => (selected.Count == 0 || !selected.Contains(pm.COD_MODALIDAD)
                                      && (string.IsNullOrEmpty(searchCriteria) || pm.DES_MODALIDAD.Contains(searchCriteria))))
                                     .Select(pm => new PurchasingModeDTO
                                     {
                                         Id = pm.COD_MODALIDAD,
                                         Name = pm.DES_MODALIDAD
                                     })
                                       .OrderBy(pm => pm.Name)
                                       .Take(pageSize)
                                       .Skip(skipRows)
                                       .ToList();

                if (PurchasingModeSelectedList != null && page == 1)
                {
                    PurchasingModeLists.AddRange(PurchasingModeSelectedList);
                }

                PurchasingModeLists.AddRange(PurchasingModeList);
                
            } catch(Exception e)
            {
                Console.WriteLine("error");
            }

            return PurchasingModeLists;
        }

    }
}
