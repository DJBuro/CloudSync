using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace CloudSyncModel.StoreDeviceModels
{
    public class StoreDevicesModels
    {
        public StoreDevicesModels() 
        {
            this.ExternalApis = new List<ExternalApiScaffold>();
            this.Devices = new List<DeviceScaffold>();
        }

        /// <summary>
        /// Gets or sets the external apis.
        /// </summary>
        /// <value>The external apis.</value>
        public List<ExternalApiScaffold> ExternalApis { get; set; }
        
        /// <summary>
        /// Gets or sets the devices.
        /// </summary>
        /// <value>The devices.</value>
        public List<DeviceScaffold> Devices { get; set; }

        /// <summary>
        /// Gets or sets the site devices.
        /// </summary>
        /// <value>The site devices.</value>
        public List<SiteDeviceScaffold> SiteDevices { get; set; }

        /// <summary>
        /// Gets or sets the removed external apis.
        /// </summary>
        /// <value>The removed external apis.</value>
        public List<ExternalApiScaffold> RemovedExternalApis { get; set; }

        /// <summary>
        /// Gets or sets the removed devices.
        /// </summary>
        /// <value>The removed devices.</value>
        public List<DeviceScaffold> RemovedDevices { get; set; }

        /// <summary>
        /// Gets or sets the removed site devices.
        /// </summary>
        /// <value>The removed site devices.</value>
        public List<SiteDeviceScaffold> RemovedSiteDevices { get; set; }
    }

    public class DeviceScaffold 
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? ExternalApiId { get; set; }
    }

    public class ExternalApiScaffold
    { 
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Parameters { get; set; }
    }

    public class SiteDeviceScaffold 
    {
        public int AndromedaSiteId { get; set; }

        public Guid DeviceId { get; set; }
        public string Parameters { get; set; }
    }

    public static class StoreDeviceModelExtensions 
    {
        public static void AddOrUpdate<TModel>(this DbSet<TModel> table, Expression<Func<TModel, bool>> queryRule,
            Func<TModel> createAction,
            Action<TModel> updateAction)

            where TModel : class 
        {
            var entity = table.Where(queryRule).SingleOrDefault();

            if (entity == null)
            {
                entity = createAction();
                table.Add(entity);
            }
            else 
            {
                updateAction(entity);
            }

        }

        public static void RemoveIfExists<TModel>(this DbSet<TModel> table,  Expression<Func<TModel, bool>> query)
            where TModel : class 
        {
            var entities = table.Where(query);

            foreach (var entity in entities) 
            {
                table.Remove(entity);
            }
        }
    
    }
}
