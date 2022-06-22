using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGCP.APPMobile.Data.DTO
{
    public class GCMResponseResult
    {
        public string message_id { get; set; }
        public string error { get; set; }
        public string registration_id { get; set; }
    }

    public class GCMResponseDTO
    {
        public string multicast_id { get; set; }
        public string success { get; set; }
        public string failure { get; set; }
        public string canonical_ids { get; set; }
        public List<GCMResponseResult> results { get; set; } 
    }
}
