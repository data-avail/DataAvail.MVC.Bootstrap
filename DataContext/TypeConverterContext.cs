using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAvail.MVC.Bootstrap.DataContext
{
    public static class TypeConverterContext
    {
        public static Action<IMappingOperationOptions> ConstructServicesUsing<TContext>(TContext DataContext, ConverterContext ConverterContext = ConverterContext.None)
        {
            return (options) => options.ConstructServicesUsing((t) =>
                GetContext(t, DataContext, ConverterContext)(t));

        }

        public static Func<Type, object> GetContext<TContext>(Type Type, TContext DataContext, ConverterContext ConverterContext)
        {
            return (p) =>
            {
                if (Type == typeof(TContext))
                {
                    return DataContext;
                }
                if (Type == typeof(ConverterContext))
                {
                    return ConverterContext;
                }
                else
                {
                    return null;
                }
            };
        }
    }

}
