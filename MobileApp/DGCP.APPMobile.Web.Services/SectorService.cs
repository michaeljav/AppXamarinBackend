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
    
    public class SectorService
    {
        private IRepository<CC_RUBROS> SectorRepository;
        private AMUoW AM;

        public SectorService()
        {
            AM = new AMUoW(new RepositoryProvider(new RepositoryFactories()));
            SectorRepository = AM.Sector;
        }

        public List<SectorDTO> GetSectors(int page = 1, List<string> selected = null, string searchCriteria = "")
        {
            List<SectorDTO> SectorList = null;
            List<SectorDTO> SectorSelectedList = null;
            List<SectorDTO> SectorLists = new List<SectorDTO>();

            try
            {
                // Pagination
                page = Convert.ToBoolean(page) ? page : 1;
                var skipRows = (page - 1) * 40;
                var pageSize = page * 40;

                if (selected.Count > 0)
                {
                    SectorSelectedList = SectorRepository.GetAll()
                                    .Where(s => selected.Contains(s.COD_RUBRO))
                                    .Select(s => new SectorDTO
                                    {
                                        Id = s.COD_RUBRO,
                                        Name = s.DES_RUBRO
                                    })
                                    .OrderBy(s => s.Name)
                                    .ToList();
                }

                SectorList = SectorRepository.GetAll()
                                     .Where(s => (selected.Count == 0 || !selected.Contains(s.COD_RUBRO))
                                      && (string.IsNullOrEmpty(searchCriteria) || s.DES_RUBRO.Contains(searchCriteria))
                                      && s.FCH_BAJA == null)
                                     .Select(s => new SectorDTO
                                     {
                                         Id = s.COD_RUBRO,
                                         Name = s.DES_RUBRO
                                     })
                                       .OrderBy(pm => pm.Name)
                                       .Take(pageSize)
                                       .Skip(skipRows)
                                       .ToList();

                if (SectorSelectedList != null && page == 1)
                {
                    SectorLists.AddRange(SectorSelectedList);
                }

                SectorLists.AddRange(SectorList);
                
            } catch(Exception e)
            {
                Console.WriteLine("error");
            }

            return SectorLists;
        }

    }
}
