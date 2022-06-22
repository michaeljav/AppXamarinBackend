using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DGCP.APPMobile.Data;
using DGCP.APPMobile.Data.UoWs;

namespace DGCP.APPMobile.Web.Services
{
    public class TransactionLogService
    {
        private IRepository<LogTransaccion> TranLogRepository { get; set; }
        private AMUoW AM;
        public TransactionLogService(AMUoW unitOfWork)
        {
            AM = unitOfWork;
            TranLogRepository = AM.TransactionLog;
        }

        public void AddTransactionLog(Dispositivo device, int transactionTypeId, DateTime registrationDate,
                                        string message, string GCMResponse = "")
        {
            try
            {
                var tLog = new LogTransaccion();
                tLog.Dispositivo = device;
                tLog.TipoTransaccionId = transactionTypeId;
                tLog.FechaRegistro = registrationDate;
                tLog.Mensaje = message;
                tLog.RespuestaGCM = GCMResponse;
                TranLogRepository.Add(tLog);

            }
            catch (Exception e)
            {
                //TODO: log Message
            }

        }

    }
}
