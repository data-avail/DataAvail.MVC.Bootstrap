using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAvail.MVC.Bootstrap.Models
{
    public class BreadCrumb
    {
        public BreadCrumb(string Name, string Link)
        {
            this.Name = Name;

            this.Link = Link;
        }

        public string Name { get; set; }

        public string Link { get; set; }

    }

    public interface IBreadCrumbs
    {
        BreadCrumb[] Crumbs { get; set; }
    }
}
