using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGCP.APPMobile.Data.Enum
{
    public enum TransactionType : int
    {
        Registration = 1,
        DeviceDisabled = 2,
        ConfigurationUpdated = 3,
        NotificationSended = 4,
        NotificationNotSended = 5
    }
}
