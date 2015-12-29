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

        public List<ExternalApiScaffold> ExternalApis { get; set; }
        public List<DeviceScaffold> Devices { get; set; }
        public List<SiteDeviceScaffold> SiteDevices { get; set; }

        public List<ExternalApiScaffold> RemovedExternalApis { get; set; }

        public List<DeviceScaffold> RemovedDevices { get; set; }

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
        public static void AddOrUpdate<TModel>(this DbSet<TModel> table, Expression<Func<TModel, bool>> query,
            Func<TModel> createAction,
            Action<TModel> update)

            where TModel : class 
        {
            var entity = table.Where(query).SingleOrDefault();

            if (entity == null)
            {
                entity = createAction();
                table.Add(entity);
            }
            else 
            {
                update(entity);
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
