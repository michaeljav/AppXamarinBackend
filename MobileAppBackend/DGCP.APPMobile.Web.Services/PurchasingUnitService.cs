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
    public class PurchasingUnitService
    {
        private IRepository<CC_UNIDAD_COMPRAS> PUnitRepository { get; set; }
        private AMUoW AM;

        public PurchasingUnitService()
        {
            AM = new AMUoW(new RepositoryProvider(new RepositoryFactories()));
            PUnitRepository = AM.PurchasingUnit;
        }

        public List<PurchasingUnitDTO> GetPurchasingUnits(int page = 1, List<string> ministrySelected = null, List<string> selected = null, string searchCriteria = "")
        {
            List<PurchasingUnitDTO> PurchasingUnitList = null;
            List<PurchasingUnitDTO> PurchasingUnitSelectedList = null;
            List<PurchasingUnitDTO> PurchasingUnitLists = new List<PurchasingUnitDTO>();

            try
            {
                // Pagination
                page = Convert.ToBoolean(page) ? page : 1;
                var skipRows = (page - 1) * 40;
                var pageSize = page * 40;



                if (selected.Count > 0 || ministrySelected.Count > 0)
                {

                    PurchasingUnitSelectedList = PUnitRepository.GetAll()
                                .Where(p => p.COD_UNIDAD_COMPRAS != "0000"
                                    && (selected.Contains(p.COD_UNIDAD_COMPRAS)))
                               .Select(p => new PurchasingUnitDTO
                               {
                                   Id = p.COD_UNIDAD_COMPRAS,
                                   Name = p.DES_UNIDAD
                               })
                                      .OrderBy(p => p.Name)
                                      .ToList();
                }



                PurchasingUnitList = PUnitRepository.GetAll()
                                .Where(p => p.COD_UNIDAD_COMPRAS != "0000"
                                && (string.IsNullOrEmpty(searchCriteria) || p.DES_UNIDAD.Contains(searchCriteria))
                                && (ministrySelected.Count == 0 || ministrySelected.Contains(p.COD_CAPITULO))
                                && (selected.Count == 0 || !selected.Contains(p.COD_UNIDAD_COMPRAS))
                                )
                                .Select(p => new PurchasingUnitDTO
                                {
                                    Id = p.COD_UNIDAD_COMPRAS,
                                    Name = p.DES_UNIDAD
                                })
                                       .OrderBy(p => p.Name)
                                       .Take(pageSize)
                                       .Skip(skipRows)
                                       .ToList();

                if (PurchasingUnitSelectedList != null && page == 1)
                {
                    PurchasingUnitLists.AddRange(PurchasingUnitSelectedList);
                }

                PurchasingUnitLists.AddRange(PurchasingUnitList);

                
            } catch(Exception e)
            {
                Console.WriteLine("Error");
            }

            return PurchasingUnitLists;

        }

    }
}
