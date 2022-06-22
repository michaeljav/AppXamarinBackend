using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using DGCP.APPMobile.Data;
using DGCP.APPMobile.Data.DTO;
using DGCP.APPMobile.Data.Helpers;
using DGCP.APPMobile.Data.UoWs;

namespace DGCP.APPMobile.Web.Services
{
    public class MinistryService
    {
        private IRepository<CL_CAPITULO> MinistryRepository { get; set; }
        private AMUoW AM;

        public MinistryService()
        {
            AM = new AMUoW(new RepositoryProvider(new RepositoryFactories()));
            MinistryRepository = AM.Ministry;
        }

        public List<MinistryDTO> GetMinistries(int page = 1, List<string> selected = null, string searchCriteria = "")
        {
            List<MinistryDTO> ministryList = null;
            List<MinistryDTO> ministrySelectedList = null;
            List<MinistryDTO> ministryLists = new List<MinistryDTO>();

            try
            {
                // Pagination
                page = Convert.ToBoolean(page) ? page : 1;
                var skipRows = (page - 1) * 40;
                var pageSize = page * 40;


                if (selected.Count > 0)
                {
                    ministrySelectedList = MinistryRepository.GetAll()
                                .Where(m => selected.Contains(m.COD_CAPITULO))
                                .Select(m => new MinistryDTO
                                {
                                    Id = m.COD_CAPITULO,
                                    Name = m.NOM_CAPITULO
                                })
                                       .OrderBy(m => m.Name)
                                       .ToList();
                }

                ministryList = MinistryRepository.GetAll()
                                .Where(m => m.COD_CAPITULO != "0000" 
                                && (selected.Count == 0 || !selected.Contains(m.COD_CAPITULO))
                                && (string.IsNullOrEmpty(searchCriteria) || m.NOM_CAPITULO.Contains(searchCriteria)))
                                .Select(m => new MinistryDTO
                                       {
                                           Id = m.COD_CAPITULO,
                                           Name = m.NOM_CAPITULO
                                       })
                                       .OrderBy(m => m.Name)
                                       .Take(pageSize)
                                       .Skip(skipRows)
                                       .ToList();

                if (ministrySelectedList != null && page == 1)
                {
                    ministryLists.AddRange(ministrySelectedList);
                }

                ministryLists.AddRange(ministryList);


            } catch(Exception e)
            {
                Console.WriteLine("error");
            }


            return ministryLists;


        }
    }
}
