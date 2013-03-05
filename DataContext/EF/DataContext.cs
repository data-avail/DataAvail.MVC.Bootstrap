using DataAvail.MVC.Bootstrap.Models;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Web;

namespace DataAvail.MVC.Bootstrap.DataContext
{
    public class DataContext<TContext, TDom, TEnty> : IDataContext<TDom>
        where TContext : ObjectContext, new()
        where TEnty : class, IEntityWithKey
        where TDom : class, new()
    {

        public DataContext()
        {
        }

        public ObjectSet<TEnty> GetTable(TContext Context)
        {
            return Context.CreateObjectSet<TEnty>();
        }

        protected virtual BreadCrumb[] GetCrumbs(TEnty Enty, TContext Context, int? ParentId)
        { 
            return new BreadCrumb[0];
        }

        protected void UpdateCrumbs(TDom Dom, TEnty Enty, TContext Context, int? ParentId)
        {
            if (Dom is IBreadCrumbs)
            {
                ((IBreadCrumbs)Dom).Crumbs = GetCrumbs(Enty, Context, ParentId); 
            }
        }

        public int Create(TDom DomItem)
        {
            using (var context = new TContext())
            {
                var entyItem = AutoMapper.Mapper.Map<TEnty>(DomItem, TypeConverterContext.ConstructServicesUsing(context));

                context.SaveChanges();

                return (int)entyItem.EntityKey.EntityKeyValues[0].Value;
            }
        }

        public int Update(TDom Order)
        {
            using (var context = new TContext())
            {
                var entyItem = AutoMapper.Mapper.Map<TEnty>(Order, TypeConverterContext.ConstructServicesUsing(context));

                context.SaveChanges();

                return (int)entyItem.EntityKey.EntityKeyValues[0].Value;
            }
        }

        public TDom[] GetAll(int? Page, object Filter)
        {
            using (var context = new TContext())
            {
                var table = GetTable(context);
                
                return AutoMapper.Mapper.Map<TDom[]>(QueryAll(context, table, Filter).ToArray(), TypeConverterContext.ConstructServicesUsing(context, ConverterContext.Index));
            }
        }

        protected virtual IQueryable<TEnty> QueryAll(TContext Context, IQueryable<TEnty> ObjectSet, object Filter)
        {
            return ObjectSet.Take(25);
        }

        public TDom GetOne(int Id)
        {
            using (var context = new TContext())
            {
                var table = GetTable(context);

                var enty = QuerySingle(GetSingle(context, table, Id));

                var item = AutoMapper.Mapper.Map<TDom>(enty, TypeConverterContext.ConstructServicesUsing(context, ConverterContext.Item));

                UpdateCrumbs(item, enty, null, null);

                return item;
            }
        }

        protected virtual TEnty QuerySingle(TEnty Enty)
        {
            return Enty;
        }


        public TDom GetNew(int? ParentId)
        {
            var item = new TDom();

            using (var context = new TContext())
            {
                InitializeDefaults(context, ParentId, item);

                UpdateCrumbs(item, null, context, ParentId);
            }

            return item;
        }

        protected virtual void InitializeDefaults(TContext Context, int? ParentId, TDom Item)
        {         
        }

        public void Delete(int Id)
        {
            using (var context = new TContext())
            {
                var table = GetTable(context);
                context.DeleteObject(GetSingle(context, table, Id));
                context.SaveChanges();
            }
        }

        private TEnty GetSingle(TContext Context, ObjectSet<TEnty> Table, object Id)
        {
            var entityKey = new System.Data.EntityKey(Context.DefaultContainerName + "." + Table.EntitySet.Name, "Id", Id);
            return (TEnty)Context.GetObjectByKey(entityKey);     
        }


        public void InitializeCrumbs(TDom Dom, int? ParentId)
        {
            using (var context = new TContext())
            {
                UpdateCrumbs(Dom, null, context, ParentId);
            }
        }
    }
}