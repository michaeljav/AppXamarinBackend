//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DGCP.APPMobile.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class LogTransaccion
    {
        public int Id { get; set; }
        public int DispositivoId { get; set; }
        public int TipoTransaccionId { get; set; }
        public System.DateTime FechaRegistro { get; set; }
        public string Mensaje { get; set; }
        public string RespuestaGCM { get; set; }
    
        public virtual TipoTransaccion TipoTransaccion { get; set; }
        public virtual Dispositivo Dispositivo { get; set; }
    }
}
