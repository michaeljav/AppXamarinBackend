using System;
using DGCP.APPMobile.Data.Helpers;
using MOnline.Data.UoWs;

namespace DGCP.APPMobile.Data.UoWs
{
    public class AMUoW : IDisposable, IUoW
    {
        public AMUoW(IRepositoryProvider repositoryProvider)
        {
            CreateDbContext();

            repositoryProvider.DbContext = DbContext;
            RepositoryProvider = repositoryProvider;       
        }

        // APPMobile repositories

        public IRepository<CC_TRAMITES_COMPRAS> Procurement { get { return GetStandardRepo<CC_TRAMITES_COMPRAS>(); } }
        public IRepository<CC_UNIDAD_COMPRAS> PurchasingUnit { get { return GetStandardRepo<CC_UNIDAD_COMPRAS>(); } }
        public IRepository<CL_CAPITULO> Ministry { get { return GetStandardRepo<CL_CAPITULO>(); } }
        public IRepository<CC_MODALIDAD_COMPRA> PurchasingMode { get { return GetStandardRepo<CC_MODALIDAD_COMPRA>(); } }
        public IRepository<CC_RUBROS> Sector { get { return GetStandardRepo<CC_RUBROS>(); } }
        public IRepository<CC_PUBLICACIONES> Publication { get { return GetStandardRepo<CC_PUBLICACIONES>(); } }
        public IRepository<CC_TRAMITE_ITEMS> ProcurementItems { get { return GetStandardRepo<CC_TRAMITE_ITEMS>(); } }
        public IRepository<CC_DOM_ESTADOS> State { get { return GetStandardRepo<CC_DOM_ESTADOS>(); } }
        public IRepository<cc_publicaciones_items> PublicationItems { get { return GetStandardRepo<cc_publicaciones_items>(); } }
        
        
        //Notification Service
        public IRepository<Dispositivo> Device { get { return GetStandardRepo<Dispositivo>(); } }
        public IRepository<ConfiguracionNotificacion> NotificationConfiguration { get { return GetStandardRepo<ConfiguracionNotificacion>(); } }
        public IRepository<LogTransaccion> TransactionLog { get { return GetStandardRepo<LogTransaccion>(); } }

        /// <summary>
        /// Save pending changes to the database
        /// </summary>
        public void Commit()
        {
            //System.Diagnostics.Debug.WriteLine("Committed");
            DbContext.SaveChanges();
        }

        protected void CreateDbContext()
        {
            DbContext = new MAppEntities();

            // Do NOT enable proxied entities, else serialization fails
            DbContext.Configuration.ProxyCreationEnabled = true;

            // Load navigation properties explicitly (avoid serialization trouble)
            DbContext.Configuration.LazyLoadingEnabled = true;

            // Because Web API will perform validation, we don't need/want EF to do so
            DbContext.Configuration.ValidateOnSaveEnabled = false;

            //DbContext.Configuration.AutoDetectChangesEnabled = false;
            // We won't use this performance tweak because we don't need 
            // the extra performance and, when autodetect is false,
            // we'd have to be careful. We're not being that careful.
        }

        protected IRepositoryProvider RepositoryProvider { get; set; }

        private IRepository<T> GetStandardRepo<T>() where T : class
        {
            return RepositoryProvider.GetRepositoryForEntityType<T>();
        }
        private T GetRepo<T>() where T : class
        {
            return RepositoryProvider.GetRepository<T>();
        }

        private MAppEntities DbContext { get; set; }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (DbContext != null)
                {
                    DbContext.Dispose();
                }
            }
        }

        #endregion
    }
}
