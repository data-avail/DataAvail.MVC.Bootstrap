using DataAvail.MVC.Bootstrap;
using DataAvail.MVC.Bootstrap.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataAvail.MVC.Bootstrap.Helpers
{
    public static class TableHelpers
    {
        public class BtpTableOpts 
        {
            public MvcHtmlString RowOpersHtml { get; set; }
        }

        public static MvcHtmlString BtpTable<TModel>(this HtmlHelper<TModel> html, BtpTableOpts Opts = null)
        {
            Type elementType = html.GetModelType<TModel>();

            var columns = elementType.GetProperties().Select(p =>
            {
                ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForProperty(null, elementType, p.Name);
                bool show = metadata.ShowForDisplay;

                object renderMode;

                if (metadata.AdditionalValues.TryGetValue("RenderMode", out renderMode))
                {
                    show = (((RenderMode)renderMode) & RenderMode.Table) != RenderMode.None;
                }

                return show ? new { metadata = metadata, name = metadata.GetDispayFieldName(p.Name), order = metadata.Order } : null;
            })
            .Where(p => p != null).OrderBy(p => p.order);


            var header = string.Join("", columns.Select(p => string.Format("<th>{0}</th>", p.name)));
            var row = columns.Select(s => string.Format("<td>{1}<span data-bind='{0}'></span></td>", GetBinding(s.metadata), GetLink(s.metadata)));
            var opers = Opts == null || Opts.RowOpersHtml == null ? BootstrapHelperResources.TableOpers : Opts.RowOpersHtml.ToString();
            opers = string.Format(opers, Resources.Text.DetailsLink);


            var table = BootstrapHelperResources.Table
                .Replace("{0}", header)
                .Replace("{1}", string.Join("", row))
                .Replace("{2}", elementType.Name)
                .Replace("{3}", opers);

            return new MvcHtmlString(table);
        }

        private static string GetLink(ModelMetadata MetaData)
        {
            object linkRes;
            object linkRef;

            if (MetaData.AdditionalValues.TryGetValue("LinkResource", out linkRes) && MetaData.AdditionalValues.TryGetValue("LinkReferenceFieldName", out linkRef))
            {
                return string.Format("<a href=\"#\" data-bind=\"link : {0}, linkOpts : {{resource : '{1}' }}\"><i class=\"icon-share\"></i></a>", linkRef, linkRes);
            }
            else
            {
                return null;
            }

        }

        private static string GetBinding(ModelMetadata MetaData)
        {
            var bind = "text";

            if (MetaData.ModelType == typeof(DateTime) || MetaData.ModelType == typeof(DateTime?))
            {
                bind = "displaydate";
            }

            return string.Format("{0} : {1}", bind, MetaData.PropertyName);
        }

    }
}