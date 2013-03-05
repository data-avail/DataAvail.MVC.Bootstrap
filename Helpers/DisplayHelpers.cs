using DataAvail.MVC.Bootstrap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataAvail.MVC.Bootstrap.Helpers
{
    public static class DisplayHelpers
    {
        public static MvcHtmlString BreadCrumbs<TModel>(this HtmlHelper<TModel> html)
        {
            IBreadCrumbs breadCrumbs = html.ViewData.Model as IBreadCrumbs;

            if (breadCrumbs != null)
            {
                var crumbs = breadCrumbs.Crumbs.Select(p =>
                {
                    var li = new TagBuilder("li");
                    var href = new TagBuilder("a");
                    href.MergeAttribute("href", p.Link);
                    href.SetInnerText(p.Name);
                    var span = new TagBuilder("span");
                    span.AddCssClass("divider");
                    span.SetInnerText("/");
                    li.InnerHtml = href.ToString(TagRenderMode.Normal) + span.ToString(TagRenderMode.Normal);
                    return li;
                });

                var ul = new TagBuilder("ul");
                ul.AddCssClass("breadcrumb");
                foreach (var crumb in crumbs)
                    ul.InnerHtml += crumb.ToString(TagRenderMode.Normal);


                return new MvcHtmlString(ul.ToString(TagRenderMode.Normal));
            }
            else
            {
                return MvcHtmlString.Empty;
            }
        }

        public static MvcHtmlString MenuItem<TModel>(this HtmlHelper<TModel> html, string Controller, string Action, string Text)
        {

            //<li class="active"><a href="/Orders/Index">Заказы</a></li>
            var li = new TagBuilder("li");
            var routeData = html.ViewContext.RouteData;
            if (routeData.GetRequiredString("controller").Equals(Controller) && routeData.GetRequiredString("action").Equals(Action))
            {
                li.AddCssClass("active");
            }
            var a = new TagBuilder("a");
            a.MergeAttribute("href", string.Format("/{0}/{1}", Controller, Action));
            a.SetInnerText(Text);
            li.InnerHtml = a.ToString(TagRenderMode.Normal);

            return new MvcHtmlString(li.ToString(TagRenderMode.Normal));
        }
    }
}