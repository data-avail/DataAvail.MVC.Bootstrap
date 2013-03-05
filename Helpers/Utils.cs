using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataAvail.MVC.Bootstrap.Helpers
{
    internal static class Utils
    {
        internal static Type GetModelType<TModel>(this HtmlHelper<TModel> html)
        {
            return GetModelType(typeof(TModel));
        }

        internal static Type GetModelType(Type TModel)
        {
            Type modelType = TModel;
            if (modelType.IsArray)
            {
                modelType = modelType.GetElementType();
            }

            return modelType;
        }


        internal static string GetDispayFieldName(this ModelMetadata metadata, string htmlFieldName)
        {
            return metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
        }

        internal static string GetDispayFieldId(this ModelMetadata metadata, string htmlFieldName, FormType FormType)
        {
            return string.Format("{0}_{1}_{2}", metadata.ContainerType.Name.Split('.').Last(), FormType.ToString().Split('.').Last(), htmlFieldName);
        }

        internal static string GetPropertyValue<TModel>(TModel Model, string PropertyName, ModelMetadata PropertyMetadata)
        {
            var val = Model.GetType().GetProperties().Single(p => p.Name == PropertyName).GetValue(Model);

            return val != null ? val.ToString() : null;
        }
    }
}