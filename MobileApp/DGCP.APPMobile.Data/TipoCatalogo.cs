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
    
    public partial class TipoCatalogo
    {
        public TipoCatalogo()
        {
            this.ConfiguracionNotificacion = new HashSet<ConfiguracionNotificacion>();
        }
    
        public int Id { get; set; }
        public string TipoCatalogo1 { get; set; }
    
        public virtual ICollection<ConfiguracionNotificacion> ConfiguracionNotificacion { get; set; }
    }
}