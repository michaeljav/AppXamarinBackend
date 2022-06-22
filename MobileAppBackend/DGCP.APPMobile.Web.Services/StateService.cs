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
    
    public class StateService
    {
        private IRepository<CC_DOM_ESTADOS> StateRepository;
        private AMUoW AM;

        public StateService()
        {
            AM = new AMUoW(new RepositoryProvider(new RepositoryFactories()));
            StateRepository = AM.State;
        }

        public List<StateDTO> GetStates(int page = 1, List<string> selected = null, string searchCriteria = "")
        {
            List<StateDTO> StateList = null;
            List<StateDTO> StateSelectedList = null;
            List<StateDTO> StateLists = new List<StateDTO>();

            try
            {
                // Pagination
                page = Convert.ToBoolean(page) ? page : 1;
                var skipRows = (page - 1) * 40;
                var pageSize = page * 40;

                if (selected.Count > 0)
                {
                    StateSelectedList = StateRepository.GetAll()
                                    .Where(s => selected.Contains(s.COD_ESTADO))
                                    .Select(s => new StateDTO
                                    {
                                        Id = s.COD_ESTADO,
                                        Name = s.DES_ESTADO
                                    })
                                    .OrderBy(s => s.Name)
                                    .ToList();
                }

                StateList = StateRepository.GetAll()
                                     .Where(s => (selected.Count == 0 || !selected.Contains(s.COD_ESTADO)
                                      && (string.IsNullOrEmpty(searchCriteria) || s.DES_ESTADO.Contains(searchCriteria))))
                                     .Select(s => new StateDTO
                                     {
                                         Id = s.COD_ESTADO,
                                         Name = s.DES_ESTADO
                                     })
                                       .OrderBy(pm => pm.Name)
                                       .Take(pageSize)
                                       .Skip(skipRows)
                                       .ToList();

                if (StateSelectedList != null && page == 1)
                {
                    StateLists.AddRange(StateSelectedList);
                }

                StateLists.AddRange(StateList);
                
            } catch(Exception e)
            {
                Console.WriteLine("error", e.Message);
            }

            return StateLists;
        }

    }
}
